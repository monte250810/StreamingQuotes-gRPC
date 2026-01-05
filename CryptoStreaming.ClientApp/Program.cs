using System.Text.Json;
using Grpc.Core;
using Grpc.Net.Client;
using Protos.Crypto;

Console.WriteLine("🚀 CryptoStreaming gRPC Client");
Console.WriteLine("================================\n");
using var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions
{
    HttpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    }
});

var client = new CryptoStreamingService.CryptoStreamingServiceClient(channel);

// 1. Get single symbol
Console.WriteLine("📊 Getting Bitcoin info...\n");
try
{
    var bitcoin = await client.GetSymbolByIdAsync(new GetSymbolByIdRequest { SymbolId = "bitcoin" });
    PrintAsset(bitcoin);
}
catch (RpcException ex)
{
    Console.WriteLine($"Error: {ex.Status.Detail}");
}

Console.WriteLine("\n📊 Fetching top 10 crypto assets...\n");
var getSymbolsRequest = new GetSymbolsRequest { Limit = 10 };
using var symbolsStream = client.GetSymbols(getSymbolsRequest);

await foreach (var asset in symbolsStream.ResponseStream.ReadAllAsync())
{
    PrintAsset(asset);
}

// 3. Stream live prices
Console.WriteLine("\n================================");
Console.WriteLine("📈 Starting live price stream (Ctrl+C to stop)...\n");

// Load symbols from JSON file
var coinsJson = await File.ReadAllTextAsync("coins.json");
var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var coinsData = JsonSerializer.Deserialize<CoinsConfig>(coinsJson, jsonOptions);
var symbols = coinsData?.Symbols ?? [];

var streamRequest = new StreamPricesRequest { IntervalMs = 15000 };
foreach (var symbol in symbols)
{
    streamRequest.SymbolIds.Add(symbol);
}

Console.WriteLine($"📡 Streaming cryptocurrencies...\n");

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    using var priceStream = client.StreamPrices(streamRequest, cancellationToken: cts.Token);

    await foreach (var update in priceStream.ResponseStream.ReadAllAsync(cts.Token))
    {
        var arrow = update.PriceChange24H >= 0 ? "▲" : "▼";
        var color = update.PriceChange24H >= 0 ? ConsoleColor.Green : ConsoleColor.Red;

        Console.ForegroundColor = color;
        Console.Write($"  {arrow} ");
        Console.ResetColor();
        Console.WriteLine($"{update.Ticker,-6} ${update.Price,12:N2} ({update.PriceChange24H:+0.00;-0.00}%)");
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("\n\n✅ Stream stopped.");
}

Console.WriteLine("\n👋 Goodbye!");

static void PrintAsset(CryptoAssetResponse asset)
{
    var arrow = asset.PriceChange24H >= 0 ? "▲" : "▼";
    Console.WriteLine($"  {asset.MarketCapRank,3}. {asset.Symbol,-6} {asset.Name,-20} ${asset.CurrentPrice,12:N2} ({arrow}{asset.PriceChange24H:0.00}%) [{asset.Trend}]");
}

record CoinsConfig(List<string> Symbols);