namespace OrderBookExecution.Core.Models;

public class Exchange
{
    public required OrderBook OrderBook { get; set; }
    public required string Id { get; set; }
    public required ExchangeBalance ExchangeBalance { get; set; }
}