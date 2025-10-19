using Library.Application.Dtos;
using Library.Domain.Entities;

namespace Library.Application.RepositoryInterfaces;

public interface IAuthorRepository : IRepository<Author>
{
    Task<List<AuthorDto>> GetAllAuthorsAsync(CancellationToken cancellationToken = default);
}
