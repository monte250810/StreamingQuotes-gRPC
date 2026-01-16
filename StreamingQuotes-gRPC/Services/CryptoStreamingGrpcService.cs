using Application.Common.DTOs;
using Application.Common.Features.Queries;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Protos.Crypto;
namespace StreamingQuotes_gRPC.Services
{
    public sealed class CryptoStreamingGrpcService : CryptoStreamingService.CryptoStreamingServiceBase
    {
        private readonly ISender _sender;
        private readonly ILogger<CryptoStreamingGrpcService> _logger;

        public CryptoStreamingGrpcService(ISender sender, ILogger<CryptoStreamingGrpcService> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public override async Task GetSymbols(
            GetSymbolsRequest request,
            IServerStreamWriter<CryptoAssetResponse> responseStream,
            ServerCallContext context)
        {
            _logger.LogInformation("GetSymbols request received with limit: {Limit}", request.Limit);

            var query = new GetAllSymbolsQuery(request.Limit);
            var result = await _sender.Send(query, context.CancellationToken);

            if (result.IsFailure)
            {
                throw new RpcException(new Status(StatusCode.Internal, result.Error.Message));
            }

            var count = 0;
            foreach (var asset in result.Value)
            {
                await responseStream.WriteAsync(MapToResponse(asset), context.CancellationToken);
                count++;
            }

            _logger.LogInformation("GetSymbols completed, sent {Count} symbols", count);
        }

        public override async Task<CryptoAssetResponse> GetSymbolById(
            GetSymbolByIdRequest request,
            ServerCallContext context)
        {
            _logger.LogInformation("GetSymbolById request for: {SymbolId}", request.SymbolId);

            if (string.IsNullOrWhiteSpace(request.SymbolId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Symbol ID is required"));
            }

            var query = new GetSymbolByIdQuery(request.SymbolId);
            var result = await _sender.Send(query, context.CancellationToken);

            if (result.IsFailure)
            {
                throw new RpcException(new Status(StatusCode.NotFound, result.Error.Message));
            }

            return MapToResponse(result.Value);
        }

        public override async Task StreamPrices(
            StreamPricesRequest request,
            IServerStreamWriter<PriceUpdateResponse> responseStream,
            ServerCallContext context)
        {
            var symbolIds = request.SymbolIds.ToList();
            var intervalMs = request.IntervalMs > 0 ? request.IntervalMs : 15000;

            _logger.LogInformation(
                "StreamPrices started for {Count} symbols with {Interval}ms interval",
                symbolIds.Count,
                intervalMs);

            var query = new StreamPricesQuery(symbolIds, intervalMs);

            try
            {
                await foreach (var update in _sender.CreateStream(query, context.CancellationToken))
                {
                    if (!update.Price.HasValue)
                        continue;

                    await responseStream.WriteAsync(MapToPriceResponse(update), context.CancellationToken);
                    _logger.LogDebug("Streamed: {Ticker} = ${Price:N2}", update.Ticker, update.Price);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("StreamPrices cancelled by client");
            }
        }

        public override async Task SubscribePrices(
            IAsyncStreamReader<SubscribeRequest> requestStream,
            IServerStreamWriter<PriceUpdateResponse> responseStream,
            ServerCallContext context)
        {
            var subscribedSymbols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);

            _logger.LogInformation("SubscribePrices bidirectional stream started");
            var subscriptionTask = ProcessSubscriptionsAsync(requestStream, subscribedSymbols, cts.Token);
            await Task.Delay(500, context.CancellationToken);

            try
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    if (subscribedSymbols.Count == 0)
                    {
                        await Task.Delay(1000, context.CancellationToken);
                        continue;
                    }

                    var currentSymbols = subscribedSymbols.ToList();
                    var query = new StreamPricesQuery(currentSymbols, 15000);

                    await foreach (var update in _sender.CreateStream(query, cts.Token))
                    {
                        if (!subscribedSymbols.Contains(update.SymbolId))
                            continue;

                        if (!update.Price.HasValue)
                            continue;

                        await responseStream.WriteAsync(MapToPriceResponse(update), context.CancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("SubscribePrices stream ended");
            }
            finally
            {
                await cts.CancelAsync();
            }
        }

        private async Task ProcessSubscriptionsAsync(
            IAsyncStreamReader<SubscribeRequest> requestStream,
            HashSet<string> subscribedSymbols,
            CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var request in requestStream.ReadAllAsync(cancellationToken))
                {
                    switch (request.ActionCase)
                    {
                        case SubscribeRequest.ActionOneofCase.Subscribe:
                            foreach (var symbol in request.Subscribe.SymbolIds)
                            {
                                subscribedSymbols.Add(symbol.ToLowerInvariant());
                                _logger.LogInformation("Subscribed to: {Symbol}", symbol);
                            }
                            break;

                        case SubscribeRequest.ActionOneofCase.Unsubscribe:
                            foreach (var symbol in request.Unsubscribe.SymbolIds)
                            {
                                subscribedSymbols.Remove(symbol.ToLowerInvariant());
                                _logger.LogInformation("Unsubscribed from: {Symbol}", symbol);
                            }
                            break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static CryptoAssetResponse MapToResponse(CryptoAssetDto dto) => new()
        {
            Id = dto.Id,
            Symbol = dto.Symbol,
            Name = dto.Name,
            CurrentPrice = (double)(dto.CurrentPrice ?? 0),
            MarketCap = (double)(dto.MarketCap ?? 0),
            Volume24H = (double)(dto.Volume24H ?? 0),
            PriceChange24H = (double)(dto.PriceChange24H ?? 0),
            High24H = (double)(dto.High24H ?? 0),
            Low24H = (double)(dto.Low24H ?? 0),
            MarketCapRank = dto.MarketCapRank,
            ImageUrl = dto.ImageUrl ?? string.Empty,
            Trend = dto.Trend,
            LastUpdated = Timestamp.FromDateTime(DateTime.SpecifyKind(dto.LastUpdatedAt, DateTimeKind.Utc))
        };

        private static PriceUpdateResponse MapToPriceResponse(PriceUpdateDto dto) => new()
        {
            SymbolId = dto.SymbolId,
            Ticker = dto.Ticker,
            Price = (double)(dto.Price ?? 0),
            PriceChange24H = (double)(dto.PriceChange24H ?? 0),
            High24H = (double)(dto.High24H ?? 0),
            Low24H = (double)(dto.Low24H ?? 0),
            Timestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(dto.Timestamp, DateTimeKind.Utc))
        };
    }
}
