using System;
using System.Collections.Generic;
using Bazaar.Common.Posts;
using MongoDB.Bson;

namespace Bazaar.MongoDbStorage.Posts
{
    public class MongoDbPostPhoto : IPostPhoto
    {
        public ObjectId _id { get; set; }       
        public string PhotoPath { get; set; }        
        public string IdentityName { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? DeletedAt { get; set; }
    }
}