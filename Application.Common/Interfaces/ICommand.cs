using Application.Behaviors.Results;
using MediatR;

namespace Application.Common.Interfaces
{
    public interface ICommand : IRequest<Result>;

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
}
