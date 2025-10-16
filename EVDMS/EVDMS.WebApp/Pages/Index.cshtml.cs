using EVDMS.DataAccess.Constants;
using EVDMS.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVDMS.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(
            ILogger<IndexModel> logger,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, RoleNames.Admin))
                    {
                        return RedirectToPage("/Admin/Approvals/Index");
                    }
                    else if (await _userManager.IsInRoleAsync(user, RoleNames.EvmStaff))
                    {
                        return RedirectToPage("/DistributionPlans/Index");
                    }
                    else if (await _userManager.IsInRoleAsync(user, RoleNames.DealerManager))
                    {
                        return RedirectToPage("/DistributionPlans/DealerBoard");
                    }
                    else if (await _userManager.IsInRoleAsync(user, RoleNames.DealerStaff))
                    {
                        return RedirectToPage("/DealerKpi/DealerProgress");
                    }
                }
            }

            return Page();
        }
    }
}
