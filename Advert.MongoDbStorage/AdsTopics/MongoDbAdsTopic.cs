using System;
using System.Collections.Generic;
using Advert.Common.AdsTopics;
using Advert.Common.Profiles;
using MongoDB.Bson;

namespace Advert.MongoDbStorage.AdsTopics
{
    public class MongoDbAdsTopic : IAdsTopic
    {
        public MongoDbAdsTopic()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }

        public bool IsDeleted { get; set; }
        public long LikesCount { get; set; }
        public long ViewsCount { get; set; }
        public long SharesCount { get; set; }
        public long CommentsCount { get; set; }       
        public long Price { get; set; }
        public string Text { get; set; }   
        public string Title { get; set; }
        public string Location { get; set; }
        public string Currency { get; set; }
        public string ProfileName { get; set; }
        public string ProfileImagePath { get; set; }
        public string NormalizedProfileName { get; set; }
        public ObjectId Id { get; set; } 
        public ObjectId ProfileId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
        public DateTime DeletedAt { get; set; }         
        public ReleaseType ReleaseType { get; set; }
        public List<string> Tags { get; set; }
    }
}