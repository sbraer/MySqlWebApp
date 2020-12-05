using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlIdentityDal;

namespace WebApp.Pages
{
	[Authorize(Roles = "USERS")]
    public class ArticleModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int? IdArticle { get; set; }

        private readonly IArticleStore _dal;
        public ArticleModel(IArticleStore dal)
        {
            _dal = dal;
        }

        public async Task OnGetAsync()
        {
            if (IdArticle.HasValue)
			{
                Message = null;
                Article = await _dal.GetArticle(IdArticle.Value);
                if (Article == null)
				{
                    Message ="Article Id not found";
				}
            }
            else
			{
                Message = "Insert ID for the article";
			}
        }

        public Article Article { get; set; }
        public string Message { get; set; }
    }
}
