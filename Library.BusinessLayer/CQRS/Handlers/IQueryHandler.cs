using MediatR;

namespace Library.BusinessLayer.CQRS.Handlers;

public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    
}
