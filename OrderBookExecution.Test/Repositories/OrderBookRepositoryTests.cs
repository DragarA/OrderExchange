using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OrderBookExecution.Core.Models;
using OrderBookExecution.Core.Repositories;

namespace OrderBookExecution.Test.Repositories;

public class OrderBookRepositoryTests
{
    private const string ConfigKey = "DataSource:FilePath";

    private static IConfiguration BuildConfig(string? filePath)
    {
        var dict = new Dictionary<string, string?>();
        if (filePath != null)
        {
            dict[ConfigKey] = filePath;
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(dict)
            .Build();
    }

    [Test]
    public void Ctor_FileDoesNotExist_ThrowsFileNotFoundException()
    {
        var nonExistingPath = Path.Combine(Path.GetTempPath(), "non-existing.json");

        Assert.That(
            () => new OrderBookRepository(nonExistingPath),
            Throws.TypeOf<FileNotFoundException>()
                .With.Message.Contains("JSON data file not found"));
    }

    [Test]
    public async Task GetExchangesDataAsync_ValidSingleLine_ParsesExchangeAndOrderBook()
    {
        var tempFile = Path.GetTempFileName();

        var exchangeId = "1548759600.25189";
        var json = "{\"AcqTime\":\"2019-01-29T11:00:00.2518854Z\",\"Bids\":[],\"Asks\":[]}";
        var line = $"{exchangeId}\t{json}";

        await File.WriteAllTextAsync(tempFile, line);

        var config = BuildConfig(tempFile);
        var repo = new OrderBookRepository(tempFile);

        try
        {
            var exchanges = await repo.GetExchangesDataAsync();
            var exchangesList = exchanges.ToList();

            Assert.That(exchangesList, Is.Not.Null);
            Assert.That(exchangesList.Count(), Is.EqualTo(1));

            var exchange = exchangesList[0];

            Assert.That(exchange.Id, Is.EqualTo(exchangeId));
            Assert.That(exchange.OrderBook, Is.Not.Null);
            Assert.That(exchange.OrderBook.Bids, Is.Not.Null);
            Assert.That(exchange.OrderBook.Asks, Is.Not.Null);
            Assert.That(exchange.OrderBook.Bids, Is.Empty);
            Assert.That(exchange.OrderBook.Asks, Is.Empty);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public async Task GetExchangesDataAsync_InvalidJson_ThrowsInvalidOperationException()
    {
        var tempFile = Path.GetTempFileName();

        var exchangeId = "1548759600.25189";
        var invalidJson = "invalid-json-data";
        var line = $"{exchangeId}\t{invalidJson}";

        await File.WriteAllTextAsync(tempFile, line);
        
        var repo = new OrderBookRepository(tempFile);

        try
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await repo.GetExchangesDataAsync());

            Assert.That(ex!.Message, Is.EqualTo("Failed to parse JSON file."));
            Assert.That(ex.InnerException, Is.Not.Null);
            Assert.That(ex.InnerException, Is.InstanceOf<System.Text.Json.JsonException>());
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public async Task GetExchangesDataAsync_InvalidLineWithoutTab_ThrowsInvalidOperationException()
    {
        var tempFile = Path.GetTempFileName();

        var badLine = "bad-line";
        await File.WriteAllTextAsync(tempFile, badLine);

        var repo = new OrderBookRepository(tempFile);

        try
        {
            var ex = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await repo.GetExchangesDataAsync());

            Assert.That(ex!.Message, Does.StartWith("Invalid line in file:"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
