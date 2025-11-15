using Microsoft.Extensions.Configuration;
using OrderBookExecution.Core.Interfaces;

namespace OrderBookExecution.Console;

public class App(IOrderBookExecutionService orderBookExecutionService, IConfiguration config)
{
    public async Task RunAsync(string[] args)
    {
        await orderBookExecutionService.GetBestAlgorithm();
    }
}