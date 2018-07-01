using System;

namespace Egghead.Models.Articles
{   
    public class Article
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public long Likes { get; set; }
        public long Dislikes { get; set; }
        public long ViewCount { get; set; }
        public long CommentsCount { get; set; }
    }
}