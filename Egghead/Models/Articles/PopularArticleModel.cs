using System;

namespace Egghead.Models.Articles
{
    public class PopularArticleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}