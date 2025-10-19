using Library.Application.CQRS;
using Library.Application.Dtos;
using Library.Application.RepositoryInterfaces;
using MediatR;

namespace Library.Application.Authors.Queries;

public class GetAllAuthorsQuery : IQuery<List<AuthorDto>>;

public class GetAllAuthorsQueryHandler(IAuthorRepository authorRepository)
    : IRequestHandler<GetAllAuthorsQuery, List<AuthorDto>>
{
    public async Task<List<AuthorDto>> Handle(GetAllAuthorsQuery query, CancellationToken cancellationToken)
    {
        var authors = await authorRepository.GetAllAuthorsAsync(cancellationToken);
        return authors;
    }
}
