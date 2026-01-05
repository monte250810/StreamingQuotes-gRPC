using Domain.Core.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace StreamingQuotes_gRPC.Interceptors
{
    public sealed class ExceptionInterceptor : Interceptor
    {
        private readonly ILogger<ExceptionInterceptor> _logger;

        public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                throw HandleException(ex, context);
            }
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
            TRequest request,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                await continuation(request, responseStream, context);
            }
            catch (Exception ex)
            {
                throw HandleException(ex, context);
            }
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                await continuation(requestStream, responseStream, context);
            }
            catch (Exception ex)
            {
                throw HandleException(ex, context);
            }
        }

        private RpcException HandleException(Exception exception, ServerCallContext context)
        {
            _logger.LogError(exception, "gRPC error in {Method}", context.Method);

            return exception switch
            {
                CryptoSymbolNotFoundException ex => new RpcException(
                    new Status(StatusCode.NotFound, ex.Message),
                    CreateMetadata(ex.Code)),

                InvalidPriceException ex => new RpcException(
                    new Status(StatusCode.InvalidArgument, ex.Message),
                    CreateMetadata(ex.Code)),

                RateLimitExceededException ex => new RpcException(
                    new Status(StatusCode.ResourceExhausted, ex.Message),
                    CreateMetadata(ex.Code, ("retry-after", ex.RetryAfter.TotalSeconds.ToString("F0")))),

                DomainException ex => new RpcException(
                    new Status(StatusCode.FailedPrecondition, ex.Message),
                    CreateMetadata(ex.Code)),

                OperationCanceledException => new RpcException(
                    new Status(StatusCode.Cancelled, "Operation was cancelled")),

                ArgumentException ex => new RpcException(
                    new Status(StatusCode.InvalidArgument, ex.Message)),

                _ => new RpcException(
                    new Status(StatusCode.Internal, "An internal error occurred"))
            };
        }

        private static Metadata CreateMetadata(string errorCode, params (string key, string value)[] additional)
        {
            var metadata = new Metadata { { "error-code", errorCode } };

            foreach (var (key, value) in additional)
            {
                metadata.Add(key, value);
            }

            return metadata;
        }
    }
}
