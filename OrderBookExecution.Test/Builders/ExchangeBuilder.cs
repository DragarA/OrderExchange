using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Test.Builders;

public class AddExchangeOrderParams(decimal price, decimal amount, OrderType orderType)
{
    public decimal Price { get; set; } = price;
    public decimal Amount { get; set; } = amount;
    public OrderType OrderType { get; set; } = orderType;
}

public class ExchangeBuilder
{
    private List<Exchange> _exchanges = new List<Exchange>();
    
    public static ExchangeBuilder Create()
    {
        return new ExchangeBuilder();
    }

    public ExchangeBuilder AddExchange(string exchangeId, List<AddExchangeOrderParams> exchangeOrderParams, ExchangeBalance? balance = null)
    {
        var bids = new List<OrderWrapper>();
        var asks = new List<OrderWrapper>();
        
        foreach (var exchangeOrderParam in exchangeOrderParams)
        {
            var order = new Order
            {
                Price = exchangeOrderParam.Price,
                Amount = exchangeOrderParam.Amount,
                Type = exchangeOrderParam.OrderType,
                Kind = OrderKind.Limit,
                Id = null,
                Time = DateTime.UtcNow
            };

            if (exchangeOrderParam.OrderType == OrderType.Buy)
            {
                bids.Add(new OrderWrapper
                {
                    Order = order
                });
            }
            else
            {
                asks.Add(new OrderWrapper
                    {
                        Order = order
                    }
                );
            }
        }

        _exchanges.Add(new Exchange
        {
            Id = exchangeId,
            OrderBook = new OrderBook
            {
                AcqTime = "",
                Bids = bids,
                Asks = asks
            }, 
            ExchangeBalance = balance ?? new ExchangeBalance
            {
                BtcBalance = 10,
                EurBalance = 10000,
            }
        });
        return this;
    }

    public List<Exchange> Build()
    {
        return _exchanges;
    }
}