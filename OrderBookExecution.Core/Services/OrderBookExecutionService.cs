using OrderBookExecution.Core.Interfaces;
using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Core.Services;

public class OrderBookExecutionService(IOrderBookRepository orderBookRepository) : IOrderBookExecutionService
{
    public async Task<ExecutionPlan?> GetBestExecutionPlanAsync(OrderParameters orderParameters)
    {
        var exchanges = await orderBookRepository.GetExchangesDataAsync();
        return CalculateBestExecutionPlan(exchanges, orderParameters);
    }

    internal ExecutionPlan? CalculateBestExecutionPlan(IEnumerable<Exchange> exchanges, OrderParameters orderParameters)
    {
        var executionPlanActions = new List<ExecutionPlanAction>();

        if (orderParameters.Amount <= 0)
            return null;

        var crossExchangeOrderList = GetCrossExchangeOrdersForType(exchanges, orderParameters.OrderType);

        var remainingAmountToFill = orderParameters.Amount;

        foreach (var exchangeOrder in crossExchangeOrderList)
        {
            var amountToFill = Math.Min(exchangeOrder.Order.Amount, remainingAmountToFill);
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
            orderList.AddRange(orderType == OrderType.Buy
                ? exchange.OrderBook.Asks.Select(orderWrapper => (exchange.Id, orderWrapper.Order))
                : exchange.OrderBook.Bids.Select(orderWrapper => (exchange.Id, orderWrapper.Order)));
        }

        return orderType == OrderType.Buy
            ? orderList.OrderBy(o => o.Order.Price)
            : orderList.OrderByDescending(o => o.Order.Price);
    }
}