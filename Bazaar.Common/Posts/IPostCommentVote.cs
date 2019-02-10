using System;
using Bazaar.Common.Posts;
using MongoDB.Bson;

namespace Bazaar.Common.Posts
{
    public interface IPostCommentVote
    {
        ObjectId _id { get; set; }       
        string IdentityName { get; set; }
        ObjectId PostId { get; set; }      
        ObjectId CommentId { get; set; }  
        VoteType VoteType { get; set; }       
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}