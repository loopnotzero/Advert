using System;

namespace Egghead.Models.Articles
{
    public class ArticleModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}