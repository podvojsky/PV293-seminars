using Library.BusinessLayer.CQRS.Queries;
using Library.BusinessLayer.Dtos;
using Library.BusinessLayer.Mappers;
using Library.DataAccess.Repositories;
using Library.DataAccess.UnitOfWork;

namespace Library.BusinessLayer.CQRS.Handlers.Query;

public class GetAllBooksQueryHandler(IBookRepository bookRepository, IAuthorRepository authorRepository, IUnitOfWork unitOfWork) : IQueryHandler<GetAllBooksQuery, List<BookDto>>
{
    public async Task<List<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await bookRepository.GetAllWithAuthorAsync();
        var bookDtos = books.Select(book => book.MapToDto()).ToList();

        return bookDtos;
    }
}
