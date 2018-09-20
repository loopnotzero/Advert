namespace Egghead.Models.Articles
{
    public class ArticlePreviewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string LikesCount { get; set; }
        public string DislikesCount { get; set; }
        public string ViewsCount { get; set; }
        public string CommentsCount { get; set; }
        public string BookmarksCount { get; set; }
        public string CreatedAt { get; set; }
    }
}