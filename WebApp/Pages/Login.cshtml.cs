using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MySqlIdentityModel;
using WebApp.Services;

namespace WebApp.Pages
{
	[AllowAnonymous]
    public class LoginModel : PageModel
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public LoginModel(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IEmailSender emailSender,
            ILogger<LoginModel> logger)
		{
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }
		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl = returnUrl ?? Url.Content("~/");

			if (ModelState.IsValid)
			{
				try
				{
					var result = await _signInManager.PasswordSignInAsync(Username, Password, false, lockoutOnFailure: false);
					if (result.Succeeded)
					{
						Message = "Login ok";
						return LocalRedirect(returnUrl);
					}
					else
					{
						Message = "Login failed";
					}
				}
				catch (Exception ex)
				{
					Message = ex.Message;
				}
			}
			else
			{
				Message = "Login failed with other errors";
				await Task.Yield();
			}

			return Page();
		}

		[BindProperty]
        [Required(ErrorMessage = "Username is required"), MaxLength(50, ErrorMessage = "50 max length")]
        public string Username { get; set; } = null!;

        [BindProperty]
        [Required(ErrorMessage = "Password is required"), MaxLength(50, ErrorMessage = "50 max length")]
        public string Password { get; set; } = null!;

        public string Message { private set; get; }
    }
}
