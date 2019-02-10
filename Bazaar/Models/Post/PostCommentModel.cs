using System.Collections.Generic;

namespace Bazaar.Models.Post
{
    public class PostCommentModel
    {
        public bool IsVoted { get; set; }
        public bool IsOwner { get; set; }
        public string PostId { get; set; }
        public string Text { get; set; }
        public string ReplyTo { get; set; }
        public string CreatedAt { get; set; }    
        public string CommentId { get; set; }        
        public string VotesCount { get; set; }
        public string ProfileName { get; set; }
        public string ProfilePhoto { get; set; }
        public List<PostCommentModel> Comments { get; set; }
    }
}