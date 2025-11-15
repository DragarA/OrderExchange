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

    public OrderBookRepository(IConfiguration config)
    {
        var path = config["DataSource:FilePath"];

        if (string.IsNullOrWhiteSpace(path))
            throw new InvalidOperationException("JSON file path is not configured.");

        _filePath = Path.GetFullPath(path);

        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"JSON data file not found: {_filePath}");        
    }
    
    public async Task<List<Exchange>> GetExchangesDataAsync()
    {
        try
        {
            var textLines = await File.ReadAllLinesAsync(_filePath);

            return textLines.Select(ParseJsonLineToExchange).ToList();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to parse JSON file.", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Failed to read file");
        }
    }

    private Exchange ParseJsonLineToExchange(string line)
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
            OrderBook = model
        };
    }
}