using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.RestApi
{
	[Authorize(Roles = "USERS")]
	[Route("api/[controller]")]
	[ApiController]
	public class ArticlesController : ControllerBase
	{
		private readonly IArticleStore _articleDal;

		public ArticlesController(IArticleStore articleDal)
		{
			_articleDal = articleDal;
		}

		[HttpGet]
		public async Task<List<Article>> Get()
		{
			return await _articleDal.GetAllArticle();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Article>> Get(int id)
		{
			var value = await _articleDal.GetArticle(id);
			if (value == null)
			{
				return NotFound();
			}

			return value;

		}

	}
}
