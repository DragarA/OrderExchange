using Microsoft.Extensions.Configuration;
using OrderBookExecution.Core.Interfaces;
using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Console;

public class App(IOrderBookExecutionService orderBookExecutionService, IConfiguration config)
{
    public async Task RunAsync(string[] args)
    {
        if (!TryParseArgs(args, out var orderType, out var amount))
        {
            System.Console.WriteLine("Usage: dotnet run <buy|sell> <amount>");
        }

        var orderParameters = new OrderParameters
        {
            Amount = amount,
            OrderType = orderType
        };
        
        var executionPlan = await orderBookExecutionService.GetBestExecutionPlanAsync(orderParameters);

        if (executionPlan == null)
            System.Console.WriteLine("No execution plan available");
        
        System.Console.WriteLine("Execution Plans:");
        foreach (var executionPlanAction in executionPlan.ExecutionPlanActions)
        {
            System.Console.WriteLine(
                $"Execute a {executionPlanAction.OrderType} on Exchange {executionPlanAction.ExchangeId} of {executionPlanAction.Amount} BTC at price of {executionPlanAction.Price} EUR");
        }

        if (!executionPlan.IsFullyFilled)
        {
            System.Console.WriteLine($"Execution Plan is not fully filled due to not enough liquidity. Unfilled amount: {executionPlan.RemainingUnfilledAmount} BTC");
        }
        
    }

    private bool TryParseArgs(string[] args, out OrderType orderType, out decimal amount)
    {
        orderType = OrderType.Buy;
        amount = 0;

        if (args.Length < 2)
            return false;

        var orderTypeArg = args[0].Trim().ToLower();
        
        switch (orderTypeArg)
        {
            case "buy":
                orderType = OrderType.Buy;
                break;
            case "sell":
                orderType = OrderType.Sell;
                break;
            default:
                return false;
        }
        
        if(!decimal.TryParse(args[1], out amount))
            return false;

        return amount > 0;
    }
}