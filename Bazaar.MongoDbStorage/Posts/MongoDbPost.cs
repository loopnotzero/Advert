﻿using System;
using System.Collections.Generic;
using Bazaar.Common.Posts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bazaar.MongoDbStorage.Posts
{
    public class MongoDbPost : IPost
    {
        public MongoDbPost()
        {
            _id = ObjectId.GenerateNewId();
            //Create indeces
        }

        [BsonId] public ObjectId _id { get; set; }
        
        [BsonElement] public bool Hidden { get; set; }
        [BsonElement] public bool IsDeleted { get; set; }
        [BsonElement] public long LikesCount { get; set; }
        [BsonElement] public long ViewsCount { get; set; }
       
        [BsonElement] public long SharesCount { get; set; }
        [BsonElement] public long CommentsCount { get; set; }
        [BsonElement] public string Text { get; set; }
        [BsonElement] public string Title { get; set; }
        [BsonElement] public string Location { get; set; }
        [BsonElement] public string Currency { get; set; }
        [BsonElement] public string ProfileName { get; set; }
        [BsonElement] public string ProfilePhoto { get; set; }
        [BsonElement] public string IdentityName { get; set; }
        [BsonElement] public decimal Price { get; set; }
        [BsonElement] public DateTime CreatedAt { get; set; }
        [BsonElement] public DateTime? UpdatedAt { get; set; }
        [BsonElement] public DateTime? DeletedAt { get; set; }
        [BsonElement] public ReleaseType ReleaseType { get; set; }
        [BsonElement] public List<string> Tags { get; set; }
    }
}