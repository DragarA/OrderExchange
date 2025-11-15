using System.Text.Json.Serialization;

namespace OrderBookExecution.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderKind
{
    Limit,
    Market
}