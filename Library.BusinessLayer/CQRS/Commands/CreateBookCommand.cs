using Library.BusinessLayer.Dtos;

namespace Library.BusinessLayer.CQRS.Commands;

public class CreateBookCommand(BookDto bookDto) : ICommand<BookDto>
{
    public BookDto BookDto { get; } = bookDto;
}
