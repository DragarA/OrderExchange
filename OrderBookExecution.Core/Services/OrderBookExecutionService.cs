using OrderBookExecution.Core.Interfaces;
using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Core.Services;

public class OrderBookExecutionService(IOrderBookRepository orderBookRepository) : IOrderBookExecutionService
{
    public async Task<ExecutionPlan> GetBestExecutionPlanAsync(OrderParameters orderParameters)
    {
        var exchanges = await orderBookRepository.GetExchangesDataAsync();
        return CalculateBestExecutionPlan(exchanges.ToList(), orderParameters);
    }

    internal ExecutionPlan CalculateBestExecutionPlan(List<Exchange> exchanges, OrderParameters orderParameters)
    {
        var executionPlanActions = new List<ExecutionPlanAction>();

        var crossExchangeOrderList = GetCrossExchangeOrdersForType(exchanges, orderParameters.OrderType);
        
        var balances = exchanges.ToDictionary(
            e => e.Id,
            e => e.ExchangeBalance
        );
        
        var remainingAmountToFill = orderParameters.Amount;

        foreach (var exchangeOrder in crossExchangeOrderList)
        {
            if (!balances.TryGetValue(exchangeOrder.ExchangeId, out var exchangeBalance))
                continue;
            
            var amountToFill = Math.Min(exchangeOrder.Order.Amount, remainingAmountToFill);

            if (orderParameters.OrderType == OrderType.Buy)
            {
                if (exchangeOrder.Order.Price <= 0)
                    continue;
                
                var maxAmountByExchangeBalance = exchangeBalance.EurBalance / exchangeOrder.Order.Price;
                
                amountToFill = Math.Min(amountToFill, maxAmountByExchangeBalance);

                if (amountToFill <= 0)
                    continue;
                
                exchangeBalance.EurBalance -= amountToFill * exchangeOrder.Order.Price;
                exchangeBalance.BtcBalance += amountToFill;
            }
            else
            {
                amountToFill = Math.Min(amountToFill, exchangeBalance.BtcBalance);

                if (amountToFill <= 0)
                    continue;
                
                exchangeBalance.EurBalance += amountToFill * exchangeOrder.Order.Price;
                exchangeBalance.BtcBalance -= amountToFill;
            }
            
            executionPlanActions.Add(new ExecutionPlanAction
            {
                Price = exchangeOrder.Order.Price,
                Amount = amountToFill,
                OrderType = orderParameters.OrderType,
                ExchangeId = exchangeOrder.ExchangeId,
            });
                
            remainingAmountToFill -= amountToFill;

            if (remainingAmountToFill <= 0)
                break;
        }

        var executionPlan = new ExecutionPlan
        {
            ExecutionPlanActions = executionPlanActions
        };

        if (remainingAmountToFill > 0)
        {
            executionPlan.RemainingUnfilledAmount = remainingAmountToFill;
        }        
        return executionPlan;
    }

    private IEnumerable<(string ExchangeId, Order Order)> GetCrossExchangeOrdersForType(IEnumerable<Exchange> exchanges, OrderType orderType) 
    {
        var orderList = new List<(string ExchangeId, Order Order)>();
        foreach (var exchange in exchanges)
        {
            if (!IsExchangeTransactionCapable(exchange.ExchangeBalance, orderType)) continue;
            
            orderList.AddRange(orderType == OrderType.Buy
                ? exchange.OrderBook.Asks.Select(orderWrapper => (exchange.Id, orderWrapper.Order))
                : exchange.OrderBook.Bids.Select(orderWrapper => (exchange.Id, orderWrapper.Order)));
        }

        return orderType == OrderType.Buy
            ? orderList.OrderBy(o => o.Order.Price)
            : orderList.OrderByDescending(o => o.Order.Price);
    }

    private bool IsExchangeTransactionCapable(ExchangeBalance exchangeBalance, OrderType orderType)
    {
        switch (orderType)
        {
            case OrderType.Buy when exchangeBalance.EurBalance <= 0:
            case OrderType.Sell when exchangeBalance.BtcBalance <= 0:
                return false;
            default:
                return true;
        }
    }
}