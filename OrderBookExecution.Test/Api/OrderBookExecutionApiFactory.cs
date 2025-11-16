using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderBookExecution.Api;
using OrderBookExecution.Core.Interfaces;
using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Test.Api;

public class OrderBookExecutionApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IOrderBookExecutionService>();
            services.AddSingleton<IOrderBookExecutionService, FakeOrderBookExecutionService>();
        });
    }
}

public class FakeOrderBookExecutionService : IOrderBookExecutionService
{
    public Task<ExecutionPlan?> GetBestExecutionPlanAsync(OrderParameters orderParameters)
    {
        var plan = new ExecutionPlan
        {
            RemainingUnfilledAmount = 0,
            ExecutionPlanActions = [
                new()
                {
                    ExchangeId = "EX1",
                    OrderType = orderParameters.OrderType,
                    Amount = 0.5m,
                    Price = 2950m
                },
                new()
                {
                    ExchangeId = "EX2",
                    OrderType = orderParameters.OrderType,
                    Amount = 0.5m,
                    Price = 2960m
                }]
        };

        return Task.FromResult(plan);
    }
}