namespace Egghead.Models.Articles
{
    public class ArticleCommentModel
    {
        public string ArticleId { get; set; }
        public string CommentId { get; set; }
        public string Text { get; set; }
        public string ReplyTo { get; set; }
        public string CreatedAt { get; set; }
    }
}