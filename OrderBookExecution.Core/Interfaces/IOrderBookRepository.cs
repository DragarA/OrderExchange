using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Core.Interfaces;

public interface IOrderBookRepository
{
    Task<List<Exchange>> GetExchangesDataAsync();
}