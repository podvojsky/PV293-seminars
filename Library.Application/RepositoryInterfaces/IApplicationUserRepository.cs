using Library.Application.Auth.Queries;
using Library.Domain.Entities;

namespace Library.Application.RepositoryInterfaces;

public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    Task<List<UserDto>> GetAllUsersAsync();
}
