using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlIdentityDal;

namespace WebApp.Pages
{
    public class ArticlesList : PageModel
    {
        private readonly IArticleStore _dal;
        public ArticlesList(IArticleStore dal)
		{
            _dal = dal;
		}

        public async Task OnGetAsync()
        {
            try
            {
                Message = null;
                Articles = await _dal.GetAllArticle();               
            }
            catch (Exception ex)
			{
                Message = ex.Message;
			}
        }

        public List<Article> Articles { get; set; }
        public string Message { get; set; }
    }
}
