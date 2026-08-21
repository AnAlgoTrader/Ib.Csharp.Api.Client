using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ib.Csharp.Api.Client.Implementation.Model;
using IBApi;

namespace Ib.Csharp.Api.Client.Implementation;

public partial class IbClient
{
    private List<ContractDetails> contractDetailsList;
    private TaskCompletionSource<List<ContractDetails>> contractDetailsTask;
    public Action<List<ContractDetails>> OnContractDetailsUpdate { get; set; }
    
    /// <summary>
    /// Gets contract data with configurable exception behavior.
    /// </summary>
    /// <param name="contract">The contract to look up</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    /// <param name="throwOnFailure">If true, throws exceptions on timeout/empty result. If false, returns null.</param>
    /// <returns>List of ContractData, or null if throwOnFailure is false and operation fails</returns>
    public async Task<List<ContractDetails>> GetContractData(Contract contract, int timeoutMs = 5000, bool throwOnFailure = true)
    {
        contractDetailsList = [];
        contractDetailsTask = new TaskCompletionSource<List<ContractDetails>>();

        _clientSocket.reqContractDetails(1010, contract);

        var timeoutTask = Task.Delay(timeoutMs);
        var completedTask = await Task.WhenAny(contractDetailsTask.Task, timeoutTask);

        if (completedTask == timeoutTask)
        {
            return throwOnFailure ? throw new TimeoutException($"GetContractData timed out after {timeoutMs}ms") : null;
        }

        var result = await contractDetailsTask.Task;
        if ((result?.Count ?? 0) == 0 && throwOnFailure)
        {
            throw new InvalidOperationException("No contract data found");
        }
        return result;
    }

    /// <summary>
    /// Gets contract data with configurable exception behavior.
    /// </summary>
    /// <param name="symbol">Contract symbol</param>
    /// <param name="secType">Security type</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    /// <param name="throwOnFailure">If true, throws exceptions on timeout/empty result. If false, returns null.</param>
    /// <returns>List of ContractData, or null if throwOnFailure is false and operation fails</returns>
    public async Task<List<ContractDetails>> GetContractData(string symbol, string secType, int timeoutMs = 5000, bool throwOnFailure = true)
    {
        contractDetailsList = [];
        contractDetailsTask = new TaskCompletionSource<List<ContractDetails>>();

        var contract = new Contract
        {
            Symbol = symbol,
            SecType = secType
        };
        _clientSocket.reqContractDetails(1010, contract);

        var timeoutTask = Task.Delay(timeoutMs);
        var completedTask = await Task.WhenAny(contractDetailsTask.Task, timeoutTask);

        if (completedTask == timeoutTask)
        {
            return throwOnFailure ? throw new TimeoutException($"GetContractData timed out after {timeoutMs}ms") : null;
        }

        var result = await contractDetailsTask.Task;
        if ((result?.Count ?? 0) == 0 && throwOnFailure)
        {
            throw new InvalidOperationException("No contract data found");
        }
        return result;
    }
    
    public void contractDetails(int reqId, ContractDetails contractDetails)
    {
        contractDetailsList.Add(contractDetails);
    }

    public void contractDetailsEnd(int reqId)
    {
        OnContractDetailsUpdate?.Invoke(contractDetailsList);
        contractDetailsTask?.TrySetResult(contractDetailsList);
    }
    
    private List<SecurityDefinitionParameter> optionDefinitionsList;
    private TaskCompletionSource<List<SecurityDefinitionParameter>> optionDefinitionsTask;
    public Action<List<SecurityDefinitionParameter>> OptionDefinitionsUpdate { get; set; }
    
    public async Task<List<SecurityDefinitionParameter>> GetOptionDefinitions(Contract contract, int timeoutMs = 10000)
    {
        optionDefinitionsList = [];
        optionDefinitionsTask = new TaskCompletionSource<List<SecurityDefinitionParameter>>();
        var exchange = contract.SecType is "FOP" or "FUT" ? contract.Exchange : string.Empty;
        _clientSocket.reqSecDefOptParams(1000, contract.Symbol, exchange, contract.SecType, contract.ConId);
        
        var timeoutTask = Task.Delay(timeoutMs);
        var completedTask = await Task.WhenAny(optionDefinitionsTask.Task, timeoutTask);
        
        if (completedTask == timeoutTask)
            throw new TimeoutException($"GetOptionDefinitions timed out after {timeoutMs}ms");
            
        return await optionDefinitionsTask.Task;
    }
    
    public void securityDefinitionOptionParameter(int reqId, string exchange, int underlyingConId, string tradingClass,
        string multiplier, HashSet<string> expirations, HashSet<double> strikes)
    {
        optionDefinitionsList.Add(new SecurityDefinitionParameter
        {
            ReqId = reqId,
            Exchange = exchange,
            UnderlyingConId = underlyingConId,
            TradingClass = tradingClass,
            Multiplier = multiplier,
            Expirations = expirations,
            Strikes = strikes
        });
    }

    public void securityDefinitionOptionParameterEnd(int reqId)
    {
        OptionDefinitionsUpdate?.Invoke(optionDefinitionsList);
        optionDefinitionsTask?.TrySetResult(optionDefinitionsList);
    }
}