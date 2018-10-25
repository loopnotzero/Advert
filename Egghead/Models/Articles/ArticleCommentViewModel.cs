using System.Collections.Generic;

namespace Egghead.Models.Articles
{
    public class ArticleCommentViewModel
    {
        public string Text { get; set; }
        public string ReplyTo { get; set; }
        public string CreatedAt { get; set; }    
        public string CommentId { get; set; }        
        public string ArticleId { get; set; }
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileImage { get; set; }
        public string VotesCount { get; set; }
        public List<ArticleCommentViewModel> Comments { get; set; }
    }
}