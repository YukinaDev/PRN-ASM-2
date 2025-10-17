using EVDMS.DataAccess.Database;
using EVDMS.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EVDMS.DataAccess.Repositories;

public class UserRepository : Repository<ApplicationUser>, IUserRepository
{
    private new readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null) return new List<ApplicationUser>();

        var userIds = await _context.UserRoles
            .Where(ur => ur.RoleId == role.Id)
            .Select(ur => ur.UserId)
            .ToListAsync();

        return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
