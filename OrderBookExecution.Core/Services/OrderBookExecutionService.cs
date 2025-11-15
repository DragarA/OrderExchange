using OrderBookExecution.Core.Interfaces;

namespace OrderBookExecution.Core.Services;

public class OrderBookExecutionService: IOrderBookExecutionService
{
    private readonly IOrderBookRepository _orderBookRepository;
    public OrderBookExecutionService(IOrderBookRepository orderBookRepository)
    {
        _orderBookRepository = orderBookRepository;
    }

    public async Task GetBestAlgorithm()
    {
        var exchanges = await _orderBookRepository.GetExchangesDataAsync();

        var a = 7;
    }
}