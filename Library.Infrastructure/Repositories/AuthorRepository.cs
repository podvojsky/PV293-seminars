using Library.Application.Dtos;
using Library.Application.RepositoryInterfaces;
using Library.DataAccess.Data;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories;

public class AuthorRepository(ApplicationDbContext context) : Repository<Author>(context), IAuthorRepository
{
    private ApplicationDbContext ApplicationDbContext => (ApplicationDbContext)Context;

    public async Task<List<AuthorDto>> GetAllAuthorsAsync(CancellationToken cancellationToken = default)
    {
        return await ApplicationDbContext.Authors
            .Select(author => new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Biography = author.Biography,
                BirthDate = author.BirthDate,
                Country = author.Country,
                TotalBooksPublished = author.TotalBooksPublished,
                LastPublishedDate = author.LastPublishedDate,
                MostPopularGenre = author.MostPopularGenre
            })
            .ToListAsync(cancellationToken);
    }
}
