using Application.Behaviors.Results;
using MediatR;

namespace Application.Common.Messaging.Interfaces
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
}
