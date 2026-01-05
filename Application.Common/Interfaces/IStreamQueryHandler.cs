using MediatR;

namespace Application.Common.Interfaces
{
    public interface IStreamQueryHandler<TQuery, TResponse> : IStreamRequestHandler<TQuery, TResponse>
        where TQuery : IStreamQuery<TResponse>;
}
