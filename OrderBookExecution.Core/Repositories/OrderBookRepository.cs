using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using OrderBookExecution.Core.Interfaces;
using OrderBookExecution.Core.Models;

namespace OrderBookExecution.Core.Repositories;

public class OrderBookRepository: IOrderBookRepository
{
    private readonly string _filePath;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public OrderBookRepository(string path)
    {
        _filePath = path;
        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"JSON data file not found: {_filePath}");        
    }
    
    public async Task<IEnumerable<Exchange>> GetExchangesDataAsync()
    {
        try
        {
            var textLines = await File.ReadAllLinesAsync(_filePath);

            var exchanges = new List<Exchange>();
            foreach (var line in textLines)
            {
                exchanges.Add(ParseJsonLineToExchange(line));
            }
            return exchanges;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to parse JSON file.", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Failed to read file", ex);
        }
    }

    public Exchange ParseJsonLineToExchange(string line)
    {
        var splitValues = line.Split('\t');

        if (splitValues.Length != 2)
        {
            throw new InvalidOperationException($"Invalid line in file: {_filePath}");
        }
                
        var model = JsonSerializer.Deserialize<OrderBook>(splitValues[1], _jsonOptions);

        if (model == null)
        {
            throw new InvalidOperationException($"Invalid line in file: {_filePath}");
        }

        return new Exchange
        {
            Id = splitValues[0],
            OrderBook = model,
            ExchangeBalance = new ExchangeBalance
            {
                BtcBalance = 1,
                EurBalance = 10000,
            }
        };
    }
}