using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderBookExecution.Core.Interfaces;
using OrderBookExecution.Core.Services;
using OrderBookExecution.Core.Repositories;

namespace OrderBookExecution.Core.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IOrderBookRepository>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return new OrderBookRepository(config);
        });

        services.AddScoped<IOrderBookExecutionService, OrderBookExecutionService>();

        return services;
    }
}
