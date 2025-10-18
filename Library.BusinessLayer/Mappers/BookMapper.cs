using Library.BusinessLayer.Dtos;
using Library.DataAccess.Entities;

namespace Library.BusinessLayer.Mappers;

public static class BookMapper
{
    public static BookDto MapToDto(this Book book, Author? author = null)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            AuthorId = book.AuthorId,
            AuthorName = author?.Name ?? "Unknown",
            ISBN = book.ISBN,
            Year = book.Year,
            Pages = book.Pages,
            Genre = book.Genre,
        };
    }

    public static Book MapToEntity(this BookDto bookDto)
    {
        return new Book
        {
            Id = bookDto.Id,
            Title = bookDto.Title,
            AuthorId = bookDto.AuthorId,
            ISBN = bookDto.ISBN,
            Year = bookDto.Year,
            Pages = bookDto.Pages,
            Genre = bookDto.Genre,
        };
    }
}
