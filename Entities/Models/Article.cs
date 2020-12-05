using System;

namespace Entities
{
	public class Article
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public string Title { get; set; } = null!;
		public string? ArticleText { get; set; }
	}
}
