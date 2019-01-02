using System.Collections.Generic;

namespace Advert.Models.Post
{
    public class PostCommentViewModel
    {
        public string PostId { get; set; }
        public string Text { get; set; }
        public string ReplyTo { get; set; }
        public string CreatedAt { get; set; }    
        public string CommentId { get; set; }        
        public string VotesCount { get; set; }
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileImagePath { get; set; }
        public List<PostCommentViewModel> Comments { get; set; }
    }
}