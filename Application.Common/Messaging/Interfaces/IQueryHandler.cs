using Application.Behaviors.Results;
using MediatR;

namespace Application.Common.Messaging.Interfaces
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
        where TQuery : IQuery<TResponse>;
}
