using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using OrderBookExecution.Api.Dto;
using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Test.Api;

public class OrderBookExecutionControllerIntegrationTests
{
    private OrderBookExecutionApiFactory _factory = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new OrderBookExecutionApiFactory();
    }

    [TearDown]
    public void TearDown()
    {
        _factory.Dispose();
    }

    [Test]
    public async Task Post_ValidRequest_ReturnsExecutionPlanDto()
    {
        var client = _factory.CreateClient();

        var request = new GetOrderBookExecutionReqDto
        {
            OrderType = OrderType.Buy,
            Amount = 1.0m
        };

        var response = await client.PostAsJsonAsync("/api/OrderBookExecution", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var body = await response.Content.ReadFromJsonAsync<GetOrderBookExecutionResDto>();

        Assert.That(body, Is.Not.Null);
        Assert.That(body.ExecutionActons.Count, Is.EqualTo(2));

        Assert.That(body.ExecutionActons.First().ExchangeId, Is.EqualTo("EX1"));
        Assert.That(body.ExecutionActons.First().OrderType, Is.EqualTo(OrderType.Buy));
        Assert.That(body.ExecutionActons.First().Amount, Is.EqualTo(0.5m));
        Assert.That(body.ExecutionActons.First().Price, Is.EqualTo(2950m));

        Assert.That(body.ExecutionActons.Last().ExchangeId, Is.EqualTo("EX2"));
        Assert.That(body.ExecutionActons.Last().OrderType, Is.EqualTo(OrderType.Buy));
        Assert.That(body.ExecutionActons.Last().Amount, Is.EqualTo(0.5m));
        Assert.That(body.ExecutionActons.Last().Price, Is.EqualTo(2960m));
    }

    [Test]
    public async Task Post_InvalidAmount_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();

        var request = new GetOrderBookExecutionReqDto
        {
            OrderType = OrderType.Buy,
            Amount = 0m
        };

        var response = await client.PostAsJsonAsync("/api/OrderBookExecution", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}
