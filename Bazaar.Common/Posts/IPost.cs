using System;
using System.Collections.Generic;
using Bazaar.Common.Posts;
using MongoDB.Bson;

namespace Bazaar.Common.Posts
{
    public interface IPost
    {
        ObjectId _id { get; set; } 
        bool Hidden { get; set; }
        bool IsDeleted { get; set; }
        long LikesCount { get; set; }
        long ViewsCount { get; set; }
        long SharesCount { get; set; }
        long CommentsCount { get; set; }       
        string Text { get; set; }   
        string Title { get; set; }
        string Location { get; set; }
        string Currency { get; set; }
        string ProfileName { get; set; }
        string ProfileImagePath { get; set; }
        decimal Price { get; set; }
        ObjectId ProfileId { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        DateTime? DeletedAt { get; set; }
        ReleaseType ReleaseType { get; set; }
        List<String> Tags { get; set; }
    }
}