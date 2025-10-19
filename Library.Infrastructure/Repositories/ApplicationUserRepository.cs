using Library.Application.Auth.Queries;
using Library.Application.RepositoryInterfaces;
using Library.DataAccess.Data;
using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories;

public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    public ApplicationUserRepository(ApplicationDbContext context) : base(context)
    {
    }

    private ApplicationDbContext ApplicationDbContext => (ApplicationDbContext)Context;

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await ApplicationDbContext.Users
            .Select(user => new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MembershipDate = user.MembershipDate,
                Roles = ApplicationDbContext.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Join(
                        ApplicationDbContext.Roles,
                        ur => ur.RoleId,
                        r => r.Id,
                        (ur, r) => r.Name!
                    )
                    .ToList()
            })
            .ToListAsync();

        return users;
    }
}
