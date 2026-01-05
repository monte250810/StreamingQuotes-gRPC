using MediatR;

namespace Application.Common.Interfaces
{
    public interface IStreamQuery<TResponse> : IStreamRequest<TResponse>;
}
