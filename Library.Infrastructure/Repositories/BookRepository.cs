using Library.Application.RepositoryInterfaces;
using Library.DataAccess.Data;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context)
    {
    }

    private ApplicationDbContext ApplicationDbContext => (ApplicationDbContext)Context;

    public async Task<IEnumerable<Book>> GetAllWithAuthorAsync()
    {
        return await Entities
            .Include(b => b.Author)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(Guid authorId)
    {
        return await Entities
            .Where(b => b.AuthorId == authorId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByGenreAsync(string genre)
    {
        return await Entities
            .Where(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase))
            .ToListAsync();
    }

    public async Task<Book?> GetBookByIsbnAsync(string isbn)
    {
        return await Entities
            .FirstOrDefaultAsync(b => b.ISBN == isbn);
    }

    public async Task<string?> GetMostPopularGenre(Guid authorId)
    {
        return await Entities
            .Where(b => b.AuthorId == authorId)
            .GroupBy(b => b.Genre)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync();
    }
}
