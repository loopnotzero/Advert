namespace Egghead.Models.Articles
{
    public class ArticleViewModel
    {
        public string Text { get; set; }
        public string Title { get; set; }
        public string ArticleId { get; set; }
        public string CreatedAt { get; set; }
        public string LikesCount { get; set; }
        public string DislikesCount { get; set; }
        public string SharesCount { get; set; }
        public string ViewsCount { get; set; }
        public string CommentsCount { get; set; }
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
    }
}