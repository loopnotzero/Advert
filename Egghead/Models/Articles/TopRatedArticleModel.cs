using System;

namespace Egghead.Models.Articles
{
    public class TopRatedArticleModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}