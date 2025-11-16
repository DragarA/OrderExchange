namespace OrderBookExecution.Core.Models;

public class ExecutionPlan
{
    public bool IsFullyFilled => RemainingUnfilledAmount is null or 0;
    public decimal? RemainingUnfilledAmount { get; set; }
    public required List<ExecutionPlanAction> ExecutionPlanActions { get; set; }
}

public class ExecutionPlanAction
{
    public required string ExchangeId { get; set; }
    public OrderType OrderType { get; set; }
    public decimal Amount { get; set; }
    public decimal Price { get; set; }
}