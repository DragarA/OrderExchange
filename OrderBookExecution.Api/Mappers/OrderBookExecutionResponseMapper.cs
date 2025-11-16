using OrderBookExecution.Api.Dto;
using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Api.Mappers;

public static class OrderBookExecutionResponseMapper
{
    public static GetOrderBookExecutionResDto? ToDto(this ExecutionPlan? executionPlan)
    {
        if (executionPlan == null) return null;
        return new GetOrderBookExecutionResDto
        {
            IsFullyFilled =  executionPlan.IsFullyFilled,
            RemainingAmountToFill = executionPlan.RemainingUnfilledAmount,
            ExecutionActons = executionPlan.ExecutionPlanActions.Select(planAction => new GetOrderBookExecutionActionDto
            {
                Price = planAction.Price,
                Amount = planAction.Amount,
                ExchangeId = planAction.ExchangeId,
                OrderType = planAction.OrderType
            }).ToList()
        };
    }
}