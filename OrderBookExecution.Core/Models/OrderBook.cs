namespace OrderBookExecution.Core.Models;

public class OrderBook
{
    public string AcqTime { get; set; }
    
    public required List<OrderWrapper> Bids { get; set; }
    public required List<OrderWrapper> Asks { get; set; }
}