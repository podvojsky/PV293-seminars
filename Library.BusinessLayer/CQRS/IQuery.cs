using MediatR;

namespace Library.BusinessLayer.CQRS;

public interface IQuery <out TResponse> : IRequest<TResponse>
{
    
}
