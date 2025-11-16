using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Core.Interfaces;

public interface IOrderBookExecutionService
{
    public Task<ExecutionPlan?> GetBestExecutionPlanAsync(OrderParameters orderParameters);
}