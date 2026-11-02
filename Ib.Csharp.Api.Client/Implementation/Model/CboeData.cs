using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ib.Csharp.Api.Client.Implementation.Model
{
    public partial class CboeData
    {
        [JsonPropertyName("timestamp")] public string Timestamp { get; init; }

        [JsonPropertyName("data")] public Data Data { get; init; }

        [JsonPropertyName("symbol")] public string Symbol { get; init; }
    }

    public class Data
    {
        [JsonPropertyName("options")] public Option[] Options { get; set; }

        [JsonPropertyName("symbol")] public string Symbol { get; set; }

        [JsonPropertyName("security_type")] public string SecurityType { get; set; }

        [JsonPropertyName("exchange_id")] public long ExchangeId { get; set; }

        [JsonPropertyName("current_price")] public double CurrentPrice { get; set; }

        [JsonPropertyName("price_change")] public double PriceChange { get; set; }

        [JsonPropertyName("price_change_percent")]
        public double PriceChangePercent { get; set; }

        [JsonPropertyName("bid")] public double Bid { get; set; }

        [JsonPropertyName("ask")] public double Ask { get; set; }

        [JsonPropertyName("bid_size")] public long BidSize { get; set; }

        [JsonPropertyName("ask_size")] public long AskSize { get; set; }

        [JsonPropertyName("open")] public double Open { get; set; }

        [JsonPropertyName("high")] public double High { get; set; }

        [JsonPropertyName("low")] public double Low { get; set; }

        [JsonPropertyName("close")] public double Close { get; set; }

        [JsonPropertyName("prev_day_close")] public double PrevDayClose { get; set; }

        [JsonPropertyName("volume")] public long Volume { get; set; }

        [JsonPropertyName("iv30")] public double Iv30 { get; set; }

        [JsonPropertyName("iv30_change")] public double Iv30Change { get; set; }

        [JsonPropertyName("iv30_change_percent")]
        public double Iv30ChangePercent { get; set; }

        [JsonPropertyName("seqno")] public long Seqno { get; set; }

        [JsonPropertyName("last_trade_time")] public string LastTradeTime { get; set; }

        [JsonPropertyName("tick")] public Tick Tick { get; set; }
    }

    public class Option
    {
        [JsonPropertyName("option")] public string OptionOption { get; set; }

        [JsonPropertyName("bid")] public double Bid { get; set; }

        [JsonPropertyName("bid_size")] public double BidSize { get; set; }

        [JsonPropertyName("ask")] public double Ask { get; set; }

        [JsonPropertyName("ask_size")] public double AskSize { get; set; }

        [JsonPropertyName("iv")] public double Iv { get; set; }

        [JsonPropertyName("open_interest")] public double OpenInterest { get; set; }

        [JsonPropertyName("volume")] public double Volume { get; set; }

        [JsonPropertyName("delta")] public double Delta { get; set; }

        [JsonPropertyName("gamma")] public double Gamma { get; set; }

        [JsonPropertyName("vega")] public double Vega { get; set; }

        [JsonPropertyName("theta")] public double Theta { get; set; }

        [JsonPropertyName("rho")] public double Rho { get; set; }

        [JsonPropertyName("theo")] public double Theo { get; set; }

        [JsonPropertyName("change")] public double Change { get; set; }

        [JsonPropertyName("open")] public double Open { get; set; }

        [JsonPropertyName("high")] public double High { get; set; }

        [JsonPropertyName("low")] public double Low { get; set; }

        [JsonPropertyName("tick")] public Tick Tick { get; set; }

        [JsonPropertyName("last_trade_price")] public double LastTradePrice { get; set; }

        [JsonPropertyName("last_trade_time")] public string LastTradeTime { get; set; }

        [JsonPropertyName("percent_change")] public double PercentChange { get; set; }

        [JsonPropertyName("prev_day_close")] public double PrevDayClose { get; set; }
    }

    public enum Tick
    {
        Down,
        NoChange,
        Up
    };

    public partial class CboeData
    {
        public static CboeData FromJson(string json) => JsonSerializer.Deserialize<CboeData>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this CboeData self) => JsonSerializer.Serialize(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.General)
        {
            Converters =
            {
                TickConverter.Singleton,
                new DateOnlyConverter(),
                new TimeOnlyConverter(),
                IsoDateTimeOffsetConverter.Singleton
            },
        };
    }

    internal class TickConverter : JsonConverter<Tick>
    {
        public override bool CanConvert(Type t) => t == typeof(Tick);

        public override Tick Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            switch (value)
            {
                case "down":
                    return Tick.Down;
                case "no_change":
                    return Tick.NoChange;
                case "up":
                    return Tick.Up;
            }

            throw new Exception("Cannot unmarshal type Tick");
        }

        public override void Write(Utf8JsonWriter writer, Tick value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case Tick.Down:
                    JsonSerializer.Serialize(writer, "down", options);
                    return;
                case Tick.NoChange:
                    JsonSerializer.Serialize(writer, "no_change", options);
                    return;
                case Tick.Up:
                    JsonSerializer.Serialize(writer, "up", options);
                    return;
            }

            throw new Exception("Cannot marshal type Tick");
        }

        public static readonly TickConverter Singleton = new TickConverter();
    }

    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        private readonly string serializationFormat;

        public DateOnlyConverter() : this(null)
        {
        }

        public DateOnlyConverter(string serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "yyyy-MM-dd";
        }

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return DateOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(serializationFormat));
    }

    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        private readonly string serializationFormat;

        public TimeOnlyConverter() : this(null)
        {
        }

        public TimeOnlyConverter(string serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "HH:mm:ss.fff";
        }

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return TimeOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(serializationFormat));
    }

    internal class IsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override bool CanConvert(Type t) => t == typeof(DateTimeOffset);

        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
        private string _dateTimeFormat;
        private CultureInfo _culture;

        public DateTimeStyles DateTimeStyles
        {
            get => _dateTimeStyles;
            set => _dateTimeStyles = value;
        }

        public string DateTimeFormat
        {
            get => _dateTimeFormat ?? string.Empty;
            set => _dateTimeFormat = (string.IsNullOrEmpty(value)) ? null : value;
        }

        public CultureInfo Culture
        {
            get => _culture ?? CultureInfo.CurrentCulture;
            set => _culture = value;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
            {
                value = value.ToUniversalTime();
            }

            var text = value.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);
            writer.WriteStringValue(text);
        }

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string dateText = reader.GetString();

            if (!string.IsNullOrEmpty(dateText))
            {
                return !string.IsNullOrEmpty(_dateTimeFormat)
                    ? DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles)
                    : DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);
            }

            return default;
        }

        public static readonly IsoDateTimeOffsetConverter Singleton = new();
    }
}