using System;
using MongoDB.Bson;

namespace Advert.Common.Posts
{
    public interface IPostComment
    {
        ObjectId _id { get; set; }
        bool IsDeleted { get; set; }
        long VotesCount { get; set; }
        string Text { get; set; }  
        string ProfileName { get; set; }
        string ProfileImagePath { get; set; }
        ObjectId ReplyTo { get; set; }
        ObjectId ProfileId { get; set; }
        ObjectId PostId { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        DateTime DeletedAt { get; set; }
    }
}