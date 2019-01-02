using System;
using MongoDB.Bson;

namespace Advert.Common.Posts
{
    public interface IPostComment
    {
        long VotesCount { get; set; }
        string Text { get; set; }  
        string ProfileName { get; set; }
        string ProfileImagePath { get; set; }
        ObjectId Id { get; set; }
        ObjectId ReplyTo { get; set; }
        ObjectId ProfileId { get; set; }
        ObjectId PostId { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime ChangedAt { get; set; }
        DateTime DeletedAt { get; set; }
    }
}