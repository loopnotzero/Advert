using System;
using Advert.Common.Posts;
using MongoDB.Bson;

namespace Advert.Common.Posts
{
    public interface IPostCommentVote
    {
        ObjectId _id { get; set; }
        ObjectId PostId { get; set; }      
        ObjectId CommentId { get; set; }  
        ObjectId ProfileId { get; set; }
        VoteType VoteType { get; set; }       
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        DateTime DeletedAt { get; set; }
    }
}