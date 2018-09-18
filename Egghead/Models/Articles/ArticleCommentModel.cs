using System;

namespace Egghead.Models.Articles
{
    public class ArticleCommentModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string ReplyTo { get; set; }
        public long VotingPoints { get; set; }
        public string CreatedAt { get; set; }
    }
}