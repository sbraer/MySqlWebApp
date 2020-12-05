using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlIdentityModel;

namespace WebApp.Pages
{
	public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
		}
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            try
            {
                await _signInManager.SignOutAsync();
                return LocalRedirect(returnUrl);
            }
            catch (Exception ex)
			{
                Message = ex.Message;
                return Page();
			}
        }

        public string Message { private set; get; }
    }
}
