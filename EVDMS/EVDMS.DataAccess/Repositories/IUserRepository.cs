using EVDMS.DataAccess.Entities;

namespace EVDMS.DataAccess.Repositories;

public interface IUserRepository : IRepository<ApplicationUser>
{
    Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName);
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
}
