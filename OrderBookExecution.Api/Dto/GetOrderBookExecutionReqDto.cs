using System.ComponentModel.DataAnnotations;
using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Api.Dto;

public class GetOrderBookExecutionReqDto
{
    [Required]
    public OrderType OrderType { get; set; }

    [Required]
    [Range(0.0001, 10000000)]
    public decimal Amount { get; set; }

}