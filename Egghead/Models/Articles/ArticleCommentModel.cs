using System;

namespace Egghead.Models.Articles
{
    public class ArticleCommentModel
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string ReplyTo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long VotingPoints { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}