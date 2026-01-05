using Application.Behaviors.Results;
using MediatR;

namespace Application.Common.Messaging.Interfaces
{
    public interface ICommand : IRequest<Result>;

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
}
