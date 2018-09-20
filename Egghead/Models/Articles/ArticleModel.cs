namespace Egghead.Models.Articles
{
    public class ArticleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public long LikesCount { get; set; }
        public long DislikesCount { get; set; }
        public long ViewsCount { get; set; }
        public long CommentsCount { get; set; }
        public long BookmarksCount { get; set; }
        public string CreatedAt { get; set; }
    }
}