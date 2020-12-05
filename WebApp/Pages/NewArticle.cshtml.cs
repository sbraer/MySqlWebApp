using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
	[Authorize(Roles = "ADMIN")]
	public class NewArticle : PageModel
	{
		[BindProperty(SupportsGet = true)]
		public int? IdArticle { get; set; }

		private readonly IArticleStore _dal;
		public NewArticle(IArticleStore dal)
		{
			_dal = dal;
		}

		public async Task OnGetAsync()
		{
			if (IdArticle.HasValue)
			{
				// Update article
				var article = await _dal.GetArticle(IdArticle.Value);
				if (article == null)
				{
					Message = "Article Id not found";
				}
				else
				{
					ArticleTitle = article.Title;
					ArticleName = article.Name;
					ArticleText = article.ArticleText;
				}
			}
		}
		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _dal.UpdateArticle(IdArticle, ArticleTitle, ArticleName, ArticleText);
					Message = "Insert or update is ok";
				}
				catch (Exception ex)
				{
					Message = ex.Message;
				}
			}
			else
			{
				await Task.Yield();
			}

			return Page();
		}

		[BindProperty]
		[Required(ErrorMessage = "Title is required"), MaxLength(50, ErrorMessage = "50 max length")]
		public string ArticleTitle { get; set; } = null!;

		[BindProperty]
		[Required(ErrorMessage = "Author is required"), MaxLength(50,ErrorMessage ="50 max length")]
		public string ArticleName { get; set; } = null!;

		[BindProperty]
		[MaxLength(10000, ErrorMessage = "10.000 max length")]
		public string ArticleText { get; set; }

		public string Message { get; set; }

	}
}
