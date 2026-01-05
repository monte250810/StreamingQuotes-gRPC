using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Diagnostics;

namespace StreamingQuotes_gRPC.Interceptors
{
    public sealed class LoggingInterceptor : Interceptor
    {
        private readonly ILogger<LoggingInterceptor> _logger;
        public LoggingInterceptor(ILogger<LoggingInterceptor> logger) => _logger = logger;

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var stopwatch = Stopwatch.StartNew();
            var method = context.Method;

            _logger.LogInformation("gRPC request started: {Method}", method);

            try
            {
                var response = await continuation(request, context);

                stopwatch.Stop();
                _logger.LogInformation(
                    "gRPC request completed: {Method} in {ElapsedMs}ms",
                    method,
                    stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "gRPC request failed: {Method} after {ElapsedMs}ms",
                    method,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
            TRequest request,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var stopwatch = Stopwatch.StartNew();
            var method = context.Method;

            _logger.LogInformation("gRPC stream started: {Method}", method);

            try
            {
                await continuation(request, responseStream, context);

                stopwatch.Stop();
                _logger.LogInformation(
                    "gRPC stream completed: {Method} after {ElapsedMs}ms",
                    method,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (OperationCanceledException)
            {
                stopwatch.Stop();
                _logger.LogInformation(
                    "gRPC stream cancelled: {Method} after {ElapsedMs}ms",
                    method,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var stopwatch = Stopwatch.StartNew();
            var method = context.Method;

            _logger.LogInformation("gRPC duplex stream started: {Method}", method);

            try
            {
                await continuation(requestStream, responseStream, context);

                stopwatch.Stop();
                _logger.LogInformation(
                    "gRPC duplex stream completed: {Method} after {ElapsedMs}ms",
                    method,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (OperationCanceledException)
            {
                stopwatch.Stop();
                _logger.LogInformation(
                    "gRPC duplex stream cancelled: {Method} after {ElapsedMs}ms",
                    method,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
