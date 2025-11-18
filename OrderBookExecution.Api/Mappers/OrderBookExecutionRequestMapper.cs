using OrderBookExecution.Api.Dto;
using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Api.Mappers;

public static class OrderBookExecutionRequestMapper
{
    public static OrderParameters ToOrderParameters(this GetOrderBookExecutionReqDto? dto)
    {
        return new OrderParameters
        {
            OrderType =  dto.OrderType,
            Amount = dto.Amount,
        };
    }
}