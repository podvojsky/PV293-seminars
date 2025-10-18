using Library.DataAccess.Data;
using Library.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Book>> GetAllWithAuthorAsync()
    {
        return await Entities
            .Include(b => b.Author)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId)
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

    private ApplicationDbContext ApplicationDbContext => (ApplicationDbContext)Context;
}
