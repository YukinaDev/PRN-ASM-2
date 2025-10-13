using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using EVDMS.DataAccess.Entities;

namespace EVDMS.WebApp.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Tài khoản")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        returnUrl ??= Url.Content("~/");

        // Clear any existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userNameOrEmail = Input.UserName?.Trim() ?? string.Empty;

        var signInResult = await _signInManager.PasswordSignInAsync(
            userNameOrEmail,
            Input.Password,
            false,
            lockoutOnFailure: false);

        if (!signInResult.Succeeded)
        {
            // allow logging in by email
            var user = await _userManager.FindByEmailAsync(userNameOrEmail);
            if (user != null)
            {
                signInResult = await _signInManager.PasswordSignInAsync(
                    user,
                    Input.Password,
                    false,
                    lockoutOnFailure: false);
            }
        }

        if (signInResult.Succeeded)
        {
            _logger.LogInformation("User logged in.");
            return LocalRedirect(returnUrl);
        }

        if (signInResult.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Tai khoan tam thoi bi khoa. Vui long thu lai sau.");
            return Page();
        }

        ModelState.AddModelError(string.Empty, "Dang nhap that bai. Vui long kiem tra thong tin va thu lai.");
        return Page();
    }
}
