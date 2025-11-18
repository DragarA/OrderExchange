using OrderBookExecution.Core.Models;
using OrderBookExecution.Core.Services;
using OrderBookExecution.Test.Builders;

namespace OrderBookExecution.Test.Services;

public class OrderBookExecutionServiceTests
{
    private readonly OrderBookExecutionService _orderBookExecutionService = new(null!);

    [Test]
    public void CalculateBestExecutionPlan_BuyOrderTypeSingleExchange_ReturnsExecutionPlan()
    {
        var exchanges = ExchangeBuilder
            .Create()
            .AddExchange("ID1",
            [
                new AddExchangeOrderParams(2960m, 1m, OrderType.Sell),
                new AddExchangeOrderParams(2950m, 1m, OrderType.Sell),
                new AddExchangeOrderParams(2970m, 1m, OrderType.Sell)
            ])
            .Build();

        var orderParameters = new OrderParameters
        {
            OrderType = OrderType.Buy,
            Amount = 1.5m
        };

        var result = _orderBookExecutionService.CalculateBestExecutionPlan(exchanges, orderParameters);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsFullyFilled, Is.True);
        Assert.That(result.RemainingUnfilledAmount, Is.Null);
        
        Assert.That(result.ExecutionPlanActions.Count, Is.EqualTo(2));
        
        Assert.That(result.ExecutionPlanActions.First().ExchangeId, Is.EqualTo("ID1"));
        Assert.That(result.ExecutionPlanActions.First().OrderType, Is.EqualTo(OrderType.Buy));
        Assert.That(result.ExecutionPlanActions.First().Amount, Is.EqualTo(1m));
        Assert.That(result.ExecutionPlanActions.First().Price, Is.EqualTo(2950m));
        
        Assert.That(result.ExecutionPlanActions.Last().ExchangeId, Is.EqualTo("ID1"));
        Assert.That(result.ExecutionPlanActions.Last().OrderType, Is.EqualTo(OrderType.Buy));
        Assert.That(result.ExecutionPlanActions.Last().Amount, Is.EqualTo(0.5m));
        Assert.That(result.ExecutionPlanActions.Last().Price, Is.EqualTo(2960m));
    }
    
    [Test]
    public void CalculateBestExecutionPlan_BuyOrderTypeMultipleExchanges_ReturnsExecutionPlan()
    {
        var exchanges = ExchangeBuilder
            .Create()
            .AddExchange("ID1",
            [
                new AddExchangeOrderParams(2960m, 1m, OrderType.Sell),
                new AddExchangeOrderParams(2950m, 1m, OrderType.Sell),
                new AddExchangeOrderParams(2970m, 1m, OrderType.Sell)
            ])
            .AddExchange("ID2",
            [
                new AddExchangeOrderParams(2940m, 1m, OrderType.Sell),
                new AddExchangeOrderParams(2955m, 1m, OrderType.Sell),
                new AddExchangeOrderParams(2975m, 1m, OrderType.Sell)
            ])
            .Build();

        var orderParameters = new OrderParameters
        {
            OrderType = OrderType.Buy,
            Amount = 2.5m
        };

        var result = _orderBookExecutionService.CalculateBestExecutionPlan(exchanges, orderParameters);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsFullyFilled, Is.True);
        Assert.That(result.RemainingUnfilledAmount, Is.Null);
        
        Assert.That(result.ExecutionPlanActions.Count, Is.EqualTo(3));
        
        Assert.That(result.ExecutionPlanActions[0].ExchangeId, Is.EqualTo("ID2"));
        Assert.That(result.ExecutionPlanActions[0].OrderType, Is.EqualTo(OrderType.Buy));
        Assert.That(result.ExecutionPlanActions[0].Amount, Is.EqualTo(1m));
        Assert.That(result.ExecutionPlanActions[0].Price, Is.EqualTo(2940m));
        
        Assert.That(result.ExecutionPlanActions[1].ExchangeId, Is.EqualTo("ID1"));
        Assert.That(result.ExecutionPlanActions[1].OrderType, Is.EqualTo(OrderType.Buy));
        Assert.That(result.ExecutionPlanActions[1].Amount, Is.EqualTo(1m));
        Assert.That(result.ExecutionPlanActions[1].Price, Is.EqualTo(2950m));
        
        Assert.That(result.ExecutionPlanActions[2].ExchangeId, Is.EqualTo("ID2"));
        Assert.That(result.ExecutionPlanActions[2].OrderType, Is.EqualTo(OrderType.Buy));
        Assert.That(result.ExecutionPlanActions[2].Amount, Is.EqualTo(0.5m));
        Assert.That(result.ExecutionPlanActions[2].Price, Is.EqualTo(2955m));
    }
    
    [Test]
    public void CalculateBestExecutionPlan_SellOrderTypeMultipleExchanges_ReturnsExecutionPlan()
    {
        var exchanges = ExchangeBuilder
            .Create()
            .AddExchange("ID1",
            [
                new AddExchangeOrderParams(2960m, 1m, OrderType.Buy),
                new AddExchangeOrderParams(2950m, 1m, OrderType.Buy),
                new AddExchangeOrderParams(2970m, 1m, OrderType.Buy)
            ])
            .AddExchange("ID2",
            [
                new AddExchangeOrderParams(2940m, 1m, OrderType.Buy),
                new AddExchangeOrderParams(2955m, 1m, OrderType.Buy),
                new AddExchangeOrderParams(2975m, 1m, OrderType.Buy)
            ])
            .Build();

        var orderParameters = new OrderParameters
        {
            OrderType = OrderType.Sell,
            Amount = 2.5m
        };

        var result = _orderBookExecutionService.CalculateBestExecutionPlan(exchanges, orderParameters);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsFullyFilled, Is.True);
        Assert.That(result.RemainingUnfilledAmount, Is.Null);
        
        Assert.That(result.ExecutionPlanActions.Count, Is.EqualTo(3));
        
        Assert.That(result.ExecutionPlanActions[0].ExchangeId, Is.EqualTo("ID2"));
        Assert.That(result.ExecutionPlanActions[0].OrderType, Is.EqualTo(OrderType.Sell));
        Assert.That(result.ExecutionPlanActions[0].Amount, Is.EqualTo(1m));
        Assert.That(result.ExecutionPlanActions[0].Price, Is.EqualTo(2975m));
        
        Assert.That(result.ExecutionPlanActions[1].ExchangeId, Is.EqualTo("ID1"));
        Assert.That(result.ExecutionPlanActions[1].OrderType, Is.EqualTo(OrderType.Sell));
        Assert.That(result.ExecutionPlanActions[1].Amount, Is.EqualTo(1m));
        Assert.That(result.ExecutionPlanActions[1].Price, Is.EqualTo(2970m));
        
        Assert.That(result.ExecutionPlanActions[2].ExchangeId, Is.EqualTo("ID1"));
        Assert.That(result.ExecutionPlanActions[2].OrderType, Is.EqualTo(OrderType.Sell));
        Assert.That(result.ExecutionPlanActions[2].Amount, Is.EqualTo(0.5m));
        Assert.That(result.ExecutionPlanActions[2].Price, Is.EqualTo(2960m));
    }
    
    [Test]
    public void CalculateBestExecutionPlan_BuyOrderTypeMultipleExchangesNotEnoughLiquidity_ReturnsExecutionPlanWithRemainingUnfilledAmount()
    {
        var exchanges = ExchangeBuilder
            .Create()
            .AddExchange("ID1",
            [
                new AddExchangeOrderParams(2960m, 1m, OrderType.Sell),
            ])
            .AddExchange("ID2",
            [
                new AddExchangeOrderParams(2940m, 1m, OrderType.Sell),
            ])
            .Build();

        var orderParameters = new OrderParameters
        {
            OrderType = OrderType.Buy,
            Amount = 4m
        };

        var result = _orderBookExecutionService.CalculateBestExecutionPlan(exchanges, orderParameters);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsFullyFilled, Is.False);
        Assert.That(result.RemainingUnfilledAmount, Is.EqualTo(2m));
        
        Assert.That(result.ExecutionPlanActions.Count, Is.EqualTo(2));
        
        Assert.That(result.ExecutionPlanActions[0].ExchangeId, Is.EqualTo("ID2"));
        Assert.That(result.ExecutionPlanActions[0].OrderType, Is.EqualTo(OrderType.Buy));
        Assert.That(result.ExecutionPlanActions[0].Amount, Is.EqualTo(1m));
        Assert.That(result.ExecutionPlanActions[0].Price, Is.EqualTo(2940m));
        
        Assert.That(result.ExecutionPlanActions[1].ExchangeId, Is.EqualTo("ID1"));
        Assert.That(result.ExecutionPlanActions[1].OrderType, Is.EqualTo(OrderType.Buy));
        Assert.That(result.ExecutionPlanActions[1].Amount, Is.EqualTo(1m));
        Assert.That(result.ExecutionPlanActions[1].Price, Is.EqualTo(2960m));
    }
    
    [Test]
    public void CalculateBestExecutionPlan_SellOrderTypeMultipleExchangesNotEnoughLiquidity_ReturnsExecutionPlanWithRemainingUnfilledAmount()
    {
        var exchanges = ExchangeBuilder
            .Create()
            .AddExchange("ID1",
            [
                new AddExchangeOrderParams(2960m, 1m, OrderType.Buy),
            ])
            .AddExchange("ID2",
            [
                new AddExchangeOrderParams(2940m, 1m, OrderType.Buy),
            ])
            .Build();

        var orderParameters = new OrderParameters
        {
            OrderType = OrderType.Sell,
            Amount = 4m
        };

        var result = _orderBookExecutionService.CalculateBestExecutionPlan(exchanges, orderParameters);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsFullyFilled, Is.False);
        Assert.That(result.RemainingUnfilledAmount, Is.EqualTo(2m));
        
        Assert.That(result.ExecutionPlanActions.Count, Is.EqualTo(2));
        
        Assert.That(result.ExecutionPlanActions[0].ExchangeId, Is.EqualTo("ID1"));
        Assert.That(result.ExecutionPlanActions[0].OrderType, Is.EqualTo(OrderType.Sell));
        Assert.That(result.ExecutionPlanActions[0].Amount, Is.EqualTo(1m));
        Assert.That(result.ExecutionPlanActions[0].Price, Is.EqualTo(2960m));
        
        Assert.That(result.ExecutionPlanActions[1].ExchangeId, Is.EqualTo("ID2"));
        Assert.That(result.ExecutionPlanActions[1].OrderType, Is.EqualTo(OrderType.Sell));
        Assert.That(result.ExecutionPlanActions[1].Amount, Is.EqualTo(1m));
        Assert.That(result.ExecutionPlanActions[1].Price, Is.EqualTo(2940m));
    }
    
    [Test]
    public void CalculateBestExecutionPlan_BuyOrderTypeMultipleExchangesNotEnoughExchangeBalance_ReturnsExecutionPlanWithRemainingUnfilledAmount()
    {
        var exchanges = ExchangeBuilder
            .Create()
            .AddExchange("ID1",
            [
                new AddExchangeOrderParams(2960m, 1m, OrderType.Sell),
            ], new ExchangeBalance
            {
                BtcBalance = 0m,
                EurBalance = 1480m,
            })
            .AddExchange("ID2",
            [
                new AddExchangeOrderParams(2940m, 1m, OrderType.Sell),
            ], new ExchangeBalance
            {
                BtcBalance = 0m,
                EurBalance = 0m,
            })
            .Build();

        var orderParameters = new OrderParameters
        {
            OrderType = OrderType.Buy,
            Amount = 4m
        };

        var result = _orderBookExecutionService.CalculateBestExecutionPlan(exchanges, orderParameters);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsFullyFilled, Is.False);
        Assert.That(result.RemainingUnfilledAmount, Is.EqualTo(3.5m));
        
        Assert.That(result.ExecutionPlanActions.Count, Is.EqualTo(1));
        
        Assert.That(result.ExecutionPlanActions.First().ExchangeId, Is.EqualTo("ID1"));
        Assert.That(result.ExecutionPlanActions.First().OrderType, Is.EqualTo(OrderType.Buy));
        Assert.That(result.ExecutionPlanActions.First().Amount, Is.EqualTo(0.5m));
        Assert.That(result.ExecutionPlanActions.First().Price, Is.EqualTo(2960m));
    }
    
    [Test]
    public void CalculateBestExecutionPlan_SellOrderTypeMultipleExchangesNotEnoughExchangeBalance_ReturnsExecutionPlanWithRemainingUnfilledAmount()
    {
        var exchanges = ExchangeBuilder
            .Create()
            .AddExchange("ID1",
            [
                new AddExchangeOrderParams(2960m, 1m, OrderType.Buy),
            ], new ExchangeBalance
            {
                BtcBalance = 0.5m,
                EurBalance = 0,
            })
            .AddExchange("ID2",
            [
                new AddExchangeOrderParams(2940m, 1m, OrderType.Buy),
            ], new ExchangeBalance
            {
                BtcBalance = 0m,
                EurBalance = 0m,
            })
            .Build();

        var orderParameters = new OrderParameters
        {
            OrderType = OrderType.Sell,
            Amount = 4m
        };

        var result = _orderBookExecutionService.CalculateBestExecutionPlan(exchanges, orderParameters);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsFullyFilled, Is.False);
        Assert.That(result.RemainingUnfilledAmount, Is.EqualTo(3.5m));
        
        Assert.That(result.ExecutionPlanActions.Count, Is.EqualTo(1));
        
        Assert.That(result.ExecutionPlanActions.First().ExchangeId, Is.EqualTo("ID1"));
        Assert.That(result.ExecutionPlanActions.First().OrderType, Is.EqualTo(OrderType.Sell));
        Assert.That(result.ExecutionPlanActions.First().Amount, Is.EqualTo(0.5m));
        Assert.That(result.ExecutionPlanActions.First().Price, Is.EqualTo(2960m));
    }
    
    [Test]
    public void CalculateBestExecutionPlan_SellOrderTypeNoOrders_ReturnsEmptyExecutionPlanWithFullRemainingUnfilledAmount()
    {
        var exchanges = ExchangeBuilder
            .Create()
            .AddExchange("ID1",
                [])
            .AddExchange("ID2",
                [])
            .Build();

        var orderParameters = new OrderParameters
        {
            OrderType = OrderType.Sell,
            Amount = 4m
        };

        var result = _orderBookExecutionService.CalculateBestExecutionPlan(exchanges, orderParameters);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsFullyFilled, Is.False);
        Assert.That(result.RemainingUnfilledAmount, Is.EqualTo(4m));
        
        Assert.That(result.ExecutionPlanActions.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void CalculateBestExecutionPlan_BuyOrderTypeNoOrders_ReturnsEmptyExecutionPlanWithFullRemainingUnfilledAmount()
    {
        var exchanges = ExchangeBuilder
            .Create()
            .AddExchange("ID1",
                [])
            .AddExchange("ID2",
                [])
            .Build();

        var orderParameters = new OrderParameters
        {
            OrderType = OrderType.Buy,
            Amount = 4m
        };

        var result = _orderBookExecutionService.CalculateBestExecutionPlan(exchanges, orderParameters);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsFullyFilled, Is.False);
        Assert.That(result.RemainingUnfilledAmount, Is.EqualTo(4m));
        
        Assert.That(result.ExecutionPlanActions.Count, Is.EqualTo(0));
    }
    
    [TestCase(-4)]
    [TestCase(0)]
    public void CalculateBestExecutionPlan_InvalidAmount_ReturnsNoExecutionPlan(decimal amount)
    {
        var exchanges = ExchangeBuilder
            .Create()
            .AddExchange("ID1",
            [
                new AddExchangeOrderParams(2960m, 1m, OrderType.Buy),
            ])
            .AddExchange("ID2",
            [
                new AddExchangeOrderParams(2940m, 1m, OrderType.Buy),
            ])
            .Build();

        var orderParameters = new OrderParameters
        {
            OrderType = OrderType.Sell,
            Amount = amount
        };

        Assert.That(
            () => _orderBookExecutionService.CalculateBestExecutionPlan(exchanges, orderParameters),
            Throws.TypeOf<ArgumentException>()
                .With.Message.Contains("Amount must be greater than zero"));

    }
}