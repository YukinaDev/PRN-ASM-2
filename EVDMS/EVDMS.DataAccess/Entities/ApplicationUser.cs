using Microsoft.AspNetCore.Identity;

namespace EVDMS.DataAccess.Entities;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int? DealerId { get; set; }
    public Dealer? Dealer { get; set; }
}
