using Microsoft.AspNetCore.Identity;

namespace EVDMS.DataAccess.Entities;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}
