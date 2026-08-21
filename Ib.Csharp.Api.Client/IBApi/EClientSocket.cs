/* Copyright (C) 2024 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace IBApi
{
    /**
     * @class EClientSocket
     * @brief TWS/Gateway client class
     * This client class contains all the available methods to communicate with IB. Up to 32 clients can be connected to a single instance of the TWS/Gateway simultaneously. From herein, the TWS/Gateway will be referred to as the Host.
     */
    public class EClientSocket : EClient, EClientMsgSink
    {
        private int port;
        private TcpClient tcpClient;

        public EClientSocket(EWrapper wrapper, EReaderSignal eReaderSignal) :
            base(wrapper) => this.eReaderSignal = eReaderSignal;

        void EClientMsgSink.serverVersion(int version, string time)
        {
            base.serverVersion = version;

            if (!useV100Plus)
            {
                if (!CheckServerVersion(MinServerVer.MIN_VERSION, ""))
                {
                    ReportUpdateTWS(IncomingMessage.NotValid, Util.CurrentTimeMillis(), "");
                    return;
                }
            }
            else if (serverVersion < Constants.MinVersion || serverVersion > Constants.MaxVersion)
            {
                wrapper.error(clientId, Util.CurrentTimeMillis(), EClientErrors.UNSUPPORTED_VERSION.Code, EClientErrors.UNSUPPORTED_VERSION.Message, "");
                return;
            }

            if (serverVersion >= 3)
            {
                if (serverVersion < MinServerVer.LINKING)
                {
                    var buf = new List<byte>();

                    buf.AddRange(Encoding.UTF8.GetBytes(clientId.ToString()));
                    buf.Add(Constants.EOL);
                    socketTransport.Send(new EMessage(buf.ToArray()));
                }
            }

            ServerTime = time;
            isConnected = true;

            if (!AsyncEConnect)
                startApi();
        }

        /**
         * @brief Establishes a connection to the designated Host. This earlier version of eConnect does not have extraAuth parameter.
         */
        public void eConnect(string host, int port, int clientId) => eConnect(host, port, clientId, false);

        protected virtual Stream createClientStream(string host, int port)
        {
            tcpClient = new TcpClient(host, port);
            SetKeepAlive(true, 2000, 100);
            tcpClient.NoDelay = true;

            return tcpClient.GetStream();
        }



        /**
         * @brief Establishes a connection to the designated Host.
         * After establishing a connection successfully, the Host will provide the next valid order id, server's current time, managed accounts and open orders among others depending on the Host version.
         * @param host the Host's IP address. Leave blank for localhost.
         * @param port the Host's port. 7496 by default for the TWS, 4001 by default on the Gateway.
         * @param clientId Every API client program requires a unique id which can be any integer. Note that up to 32 clients can be connected simultaneously to a single Host.
         * @sa EWrapper, EWrapper::nextValidId, EWrapper::currentTime
         */
        public void eConnect(string host, int port, int clientId, bool extraAuth)
        {
            try
            {
                validateInvalidSymbols(host);
            }
            catch (EClientException e)
            {
                wrapper.error(IncomingMessage.NotValid, Util.CurrentTimeMillis(), e.Err.Code, e.Err.Message + e.Text, "");
                return;
            }

            if (isConnected)
            {
                wrapper.error(IncomingMessage.NotValid, Util.CurrentTimeMillis(), EClientErrors.AlreadyConnected.Code, EClientErrors.AlreadyConnected.Message, "");
                return;
            }
            try
            {
                tcpStream = createClientStream(host, port);
                this.port = port;
                socketTransport = new ESocket(tcpStream);

                this.clientId = clientId;
                this.extraAuth = extraAuth;

                sendConnectRequest();

                if (!AsyncEConnect)
                {
                    var eReader = new EReader(this, eReaderSignal);

                    while (serverVersion == 0 && eReader.putMessageToQueue())
                    {
                        eReaderSignal.waitForSignal();
                        eReader.processMsgs();
                    }
                }
            }
            catch (ArgumentNullException ane)
            {
                wrapper.error(ane);
            }
            catch (SocketException se)
            {
                wrapper.error(se);
            }
            catch (EClientException e)
            {
                var cmp = e.Err;

                wrapper.error(-1, Util.CurrentTimeMillis(), cmp.Code, cmp.Message, "");
            }
            catch (Exception e)
            {
                wrapper.error(e);
            }
        }

        private readonly EReaderSignal eReaderSignal;

        protected override uint prepareBuffer(BinaryWriter paramsList)
        {
            var rval = (uint)paramsList.BaseStream.Position;

            if (useV100Plus)
                paramsList.Write(0);

            return rval;
        }

        protected override void CloseAndSend(BinaryWriter request, uint lengthPos)
        {
            if (useV100Plus)
            {
                request.Seek((int)lengthPos, SeekOrigin.Begin);
                request.Write(IPAddress.HostToNetworkOrder((int)(request.BaseStream.Length - lengthPos - sizeof(int))));
            }

            request.Seek(0, SeekOrigin.Begin);

            var buf = new MemoryStream();
            try
            {
                request.BaseStream.CopyTo(buf);
                socketTransport.Send(new EMessage(buf.ToArray()));
            }
            finally
            {
                buf.Dispose();
            }
        }

        public override void eDisconnect(bool resetState = true)
        {
            tcpClient = null;
            base.eDisconnect(resetState);
        }

        /**
         * @brief Sets TCP keep-alive options on the socket connection with cross-platform and cross-framework support.
         * @param on Whether to enable keep-alive
         * @param keepAliveTime Time in milliseconds before the first keep-alive packet is sent
         * @param keepAliveInterval Time in milliseconds between keep-alive packets
         */
        private void SetKeepAlive(bool on, uint keepAliveTime, uint keepAliveInterval)
        {
            // Set basic keep-alive option which works on all platforms and frameworks
            tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, on);

            if (!on)
                return; // Keep-alive disabled, no need for further configuration

            // Try multiple approaches to set advanced keep-alive options
            bool success = TrySetKeepAliveModernApproach(keepAliveTime, keepAliveInterval) || TrySetKeepAliveIOControlApproach(keepAliveTime, keepAliveInterval);

            // If all approaches failed, at least basic keep-alive is still enabled
            if (!success)
            {
                // Optionally log that only basic keep-alive is active
            }
        }

        /**
         * @brief Attempts to set TCP keep-alive options using direct socket options (works on newer .NET versions)
         */
        private bool TrySetKeepAliveModernApproach(uint keepAliveTime, uint keepAliveInterval)
        {
            try
            {
                // Define the TCP level and options with hardcoded values to ensure compatibility
                const SocketOptionLevel TcpLevel = (SocketOptionLevel)6; // SocketOptionLevel.Tcp
                const SocketOptionName TcpKeepAliveTimeOption = (SocketOptionName)3; // SocketOptionName.TcpKeepAliveTime
                const SocketOptionName TcpKeepAliveIntervalOption = (SocketOptionName)17; // SocketOptionName.TcpKeepAliveInterval
                const SocketOptionName TcpKeepAliveRetryCountOption = (SocketOptionName)16; // SocketOptionName.TcpKeepAliveRetryCount

                // Convert milliseconds to seconds for Linux/Unix systems and ensure minimum of 1 second
                int keepAliveTimeSec = Math.Max((int)(keepAliveTime / 1000), 1);
                int keepAliveIntervalSec = Math.Max((int)(keepAliveInterval / 1000), 1);
                const int defaultRetryCount = 5;

                bool anyOptionSet = false;

                // Try setting each option individually to handle partial support scenarios
                try
                {
                    tcpClient.Client.SetSocketOption(TcpLevel, TcpKeepAliveTimeOption, keepAliveTimeSec);
                    anyOptionSet = true;
                }
                catch { /* Ignore if this specific option isn't supported */ }

                try
                {
                    tcpClient.Client.SetSocketOption(TcpLevel, TcpKeepAliveIntervalOption, keepAliveIntervalSec);
                    anyOptionSet = true;
                }
                catch { /* Ignore if this specific option isn't supported */ }

                try
                {
                    tcpClient.Client.SetSocketOption(TcpLevel, TcpKeepAliveRetryCountOption, defaultRetryCount);
                    anyOptionSet = true;
                }
                catch { /* Ignore if this specific option isn't supported */ }

                return anyOptionSet;
            }
            catch
            {
                return false; // This approach not supported, try the next one
            }
        }

        /**
         * @brief Attempts to set TCP keep-alive options using Windows-specific IOControl (works on .NET Framework and Windows)
         */
        private bool TrySetKeepAliveIOControlApproach(uint keepAliveTime, uint keepAliveInterval)
        {
            try
            {
                // SIO_KEEPALIVE_VALS = IOC_IN | IOC_VENDOR | 4 = 0x98000004
                const int SioKeepAliveValues = unchecked((int)0x98000004);

                // Calculate size of uint for buffer allocation
                int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint));
                byte[] inOptionValues = new byte[size * 3];

                // Enable keep-alive (1 = true)
                BitConverter.GetBytes(1u).CopyTo(inOptionValues, 0);

                // Keep-alive time in milliseconds
                BitConverter.GetBytes(keepAliveTime).CopyTo(inOptionValues, size);

                // Keep-alive interval in milliseconds
                BitConverter.GetBytes(keepAliveInterval).CopyTo(inOptionValues, size * 2);

                // Apply the settings
                tcpClient.Client.IOControl(SioKeepAliveValues, inOptionValues, null);
                return true;
            }
            catch
            {
                return false; // This approach not supported
            }
        }

        /**
         * @brief Determines the status of the tcpClient with SelectMode.SelectRead.
         * @param timeout The time to wait for a response, in microseconds.
         * @returns true if any of the following conditions occur before the timeout expires, otherwise, false.
         * @sa EWrapper::connectionClosed
         */
        internal bool Poll(int timeout)
        {
            return tcpClient.Client.Poll(timeout, SelectMode.SelectRead);
        }
    }
}
