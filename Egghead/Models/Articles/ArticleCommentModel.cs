using System;

namespace Egghead.Models.Articles
{
    public class ArticleCommentModel
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string ReplyTo { get; set; }
        public int Level { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}