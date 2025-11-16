using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderBookExecution.Api.Dto;
using OrderBookExecution.Api.Mappers;
using OrderBookExecution.Core.Interfaces;
using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderBookExecutionController(IOrderBookExecutionService orderBookExecutionService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<List<GetOrderBookExecutionResDto>>> Post([FromBody] GetOrderBookExecutionReqDto orderBookExecutionReq)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var orderParams = new OrderParameters
        {
            Amount = orderBookExecutionReq.Amount,
            OrderType = orderBookExecutionReq.OrderType,
        };

        var executionPlan = await orderBookExecutionService.GetBestExecutionPlanAsync(orderParams);
        
        if(executionPlan == null)
            return BadRequest();

        return Ok(executionPlan.ToDto());
    }
}