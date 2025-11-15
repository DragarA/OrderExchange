using Microsoft.AspNetCore.Mvc;

namespace OrderBookExecution.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderBookExecutionController : ControllerBase
{
    public OrderBookExecutionController()
    {
    }

    [HttpGet]
    public void Get()
    {
    }
}