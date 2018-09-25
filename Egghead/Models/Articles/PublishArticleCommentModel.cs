namespace Egghead.Models.Articles
{
    public class PublishArticleCommentModel
    {
        public string Text { get; set; }
        public string ArticleId { get; set; }
        public string ReplyTo { get; set; }
        
    }
}