using System.Collections.Generic;
using System.Threading.Tasks;

namespace Entities
{
	public interface IArticleStore
	{
		Task<List<Article>> GetAllArticle();
		Task<Article> GetArticle(int id);
		Task UpdateArticle(int? id, string title, string name, string articletext);
	}
}
