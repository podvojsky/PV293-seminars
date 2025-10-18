using Library.BusinessLayer.CQRS.Commands;
using Library.BusinessLayer.CQRS.Events;
using Library.BusinessLayer.Dtos;
using Library.BusinessLayer.Mappers;
using Library.DataAccess.Repositories;
using Library.DataAccess.UnitOfWork;
using MediatR;

namespace Library.BusinessLayer.CQRS.Handlers.Command;

public class CreateBookCommandHandler(IAuthorRepository authorRepository, IBookRepository bookRepository, IUnitOfWork unitOfWork, IMediator mediator) : ICommandHandler<CreateBookCommand, BookDto>
{
    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate author exists
        var author = await authorRepository.GetByIdAsync(request.BookDto.AuthorId);
        if (author == null)
            throw new ArgumentException($"Author with ID {request.BookDto.AuthorId} not found", nameof(request.BookDto.AuthorId));

        // 2. Check for duplicate ISBN
        var existingBooks = await bookRepository.GetAllAsync();
        if (existingBooks.Any(b => b.ISBN == request.BookDto.ISBN))
            throw new ArgumentException($"Book with ISBN {request.BookDto.ISBN} already exists", nameof(request.BookDto.ISBN));

        // 3. Create the book
        var book = request.BookDto.MapToEntity();
        bookRepository.Add(book);

        await unitOfWork.CompleteAsync();

        await mediator.Publish(new BookCreatedEvent(book.Id, book.AuthorId), cancellationToken);

        return book.MapToDto(author);
    }
}
