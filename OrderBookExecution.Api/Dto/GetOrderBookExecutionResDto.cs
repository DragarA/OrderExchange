using System.ComponentModel.DataAnnotations;
using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Api.Dto;

public class GetOrderBookExecutionResDto
{
    public List<GetOrderBookExecutionActionDto> ExecutionActons { get; set; }
    public bool IsFullyFilled { get; set; }
    public decimal? RemainingAmountToFill { get; set; }
}

public class GetOrderBookExecutionActionDto
{
    public OrderType OrderType { get; set; }
    public string ExchangeId { get; set; }
    public decimal Amount { get; set; }
    public decimal Price { get; set; }
}