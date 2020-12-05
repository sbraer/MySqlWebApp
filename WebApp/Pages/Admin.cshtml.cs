using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
	[Authorize(Roles ="ADMIN")]
    public class AdminModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
