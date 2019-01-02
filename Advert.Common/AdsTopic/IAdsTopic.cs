using System;
using System.Collections.Generic;
using Advert.Common.AdsTopics;
using MongoDB.Bson;

namespace Advert.Common.Profiles
{
    public interface IAdsTopic
    {
        bool IsDeleted { get; set; }
        long LikesCount { get; set; }
        long ViewsCount { get; set; }
        long SharesCount { get; set; }
        long CommentsCount { get; set; }       
        long Price { get; set; }
        string Text { get; set; }   
        string Title { get; set; }
        string Location { get; set; }
        string Currency { get; set; }
        string ProfileName { get; set; }
        string ProfileImagePath { get; set; }
        string NormalizedProfileName { get; set; }
        ObjectId Id { get; set; } 
        ObjectId ProfileId { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime ChangedAt { get; set; }
        DateTime DeletedAt { get; set; }
        ReleaseType ReleaseType { get; set; }
        List<String> Tags { get; set; }
    }
}