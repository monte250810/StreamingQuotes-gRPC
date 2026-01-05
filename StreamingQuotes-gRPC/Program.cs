using Application.Common.Config;
using Infrastructure.Caching.Config;
using Infrastructure.CoinGegko.Config;
using Serilog;
using Serilog.Events;
using StreamingQuotes_gRPC.Interceptors;
using StreamingQuotes_gRPC.Options;
using StreamingQuotes_gRPC.Services;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Grpc", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting CryptoStreaming gRPC Service");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Configuration sources
    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables("CRYPTO_")
        .AddUserSecrets<Program>(optional: true);

    // Get gRPC options
    var grpcOptions = builder.Configuration
        .GetSection(GrpcOptions.SectionName)
        .Get<GrpcOptions>() ?? new GrpcOptions();

    // Configure gRPC with interceptors
    builder.Services.AddGrpc(options =>
    {
        options.EnableDetailedErrors = grpcOptions.EnableDetailedErrors || builder.Environment.IsDevelopment();
        options.MaxReceiveMessageSize = grpcOptions.MaxReceiveMessageSizeMb * 1024 * 1024;
        options.MaxSendMessageSize = grpcOptions.MaxSendMessageSizeMb * 1024 * 1024;

        options.Interceptors.Add<LoggingInterceptor>();
        options.Interceptors.Add<ExceptionInterceptor>();

        if (grpcOptions.EnableMessageCompression)
        {
            options.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
            options.ResponseCompressionAlgorithm = "gzip";
        }
    });

    builder.Services.AddGrpcReflection();

    builder.Services.AddApplication();
    builder.Services.AddInfrastructureCoinGecko(builder.Configuration);
    builder.Services.AddInfrastructureCaching(builder.Configuration);

    builder.Services.AddHealthChecks()
        .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
        .AddUrlGroup(new Uri("https://api.coingecko.com/api/v3/ping"), "coingecko-api");

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "CryptoStreaming gRPC API",
            Version = "v1",
            Description = "Real-time cryptocurrency streaming service using gRPC"
        });
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    app.UseRouting();

    app.MapGrpcService<CryptoStreamingGrpcService>();

    if (app.Environment.IsDevelopment())
    {
        app.MapGrpcReflectionService();
    }

    app.MapHealthChecks("/health");

    app.MapGet("/", () => Results.Ok(new
    {
        Service = "CryptoStreaming gRPC Service",
        Version = "1.0.0",
        Status = "Running",
        Environment = app.Environment.EnvironmentName,
        Endpoints = new
        {
            Swagger = "/swagger",
            Health = "/health",
            GrpcReflection = app.Environment.IsDevelopment() ? "Enabled" : "Disabled"
        },
        GrpcServices = new[]
        {
            "crypto.CryptoStreamingService.GetSymbols",
            "crypto.CryptoStreamingService.GetSymbolById",
            "crypto.CryptoStreamingService.StreamPrices",
            "crypto.CryptoStreamingService.SubscribePrices",
            "health.HealthService.Check",
            "health.HealthService.Watch"
        }
    }));

    Log.Information("Application started successfully");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}