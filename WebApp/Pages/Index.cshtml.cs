using Configuration;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;

		public IndexModel(ILogger<IndexModel> logger, IConfigurationManager configurationManager)
		{
			_logger = logger;
			Message = configurationManager.GetIdentifier();
		}

		public void OnGet()
		{
		}

		public string Message { get; protected set; }
	}
}
