using Library.Application.CQRS;
using Library.Application.RepositoryInterfaces;
using MediatR;

namespace Library.Application.Auth.Queries;

public class GetAllUsersQuery : IQuery<List<UserDto>>
{
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime MembershipDate { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class GetAllUsersQueryHandler(IApplicationUserRepository applicationUserRepository)
    : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await applicationUserRepository.GetAllUsersAsync();

        return users;
    }
}
