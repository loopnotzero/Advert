using System;
using MongoDB.Bson;

namespace Bazaar.Common.Posts
{
    public interface IPostComment
    {
        ObjectId _id { get; set; }
        bool IsDeleted { get; set; }
        long VotesCount { get; set; }
        string Text { get; set; }  
        string ProfileName { get; set; }
        string ProfilePhoto { get; set; }
        string IdentityName { get; set; }
        ObjectId PostId { get; set; }
        ObjectId ReplyTo { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}