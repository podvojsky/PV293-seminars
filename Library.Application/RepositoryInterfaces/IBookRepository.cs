using Library.Domain.Entities;

namespace Library.Application.RepositoryInterfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(Guid authorId);
    Task<IEnumerable<Book>> GetBooksByGenreAsync(string genre);
    Task<Book?> GetBookByIsbnAsync(string isbn);
    Task<string?> GetMostPopularGenre(Guid authorId);
    Task<IEnumerable<Book>> GetAllWithAuthorAsync();
}
