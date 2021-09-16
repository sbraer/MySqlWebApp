using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.CustomPolicyProvider;

namespace WebApp.Pages
{
	//[Authorize]
	[SecurityLevel(90)]
	public class PrivacyModel : PageModel
	{
		private readonly ILogger<PrivacyModel> _logger;

		public PrivacyModel(ILogger<PrivacyModel> logger)
		{
			_logger = logger;
		}

		public void OnGet()
		{
			if (User.Identity.IsAuthenticated)
            {
				ClaimInfo = string.Empty;
				User.Claims.ToList().ForEach(t =>
				{
					ClaimInfo += $"{t.Type}: {t.Value}<br />";
				});
            }
			else
            {
				ClaimInfo = string.Empty;
            }
		}

		public string ClaimInfo {  get; set; }
	}
}
