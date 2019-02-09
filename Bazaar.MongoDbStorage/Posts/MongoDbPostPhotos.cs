using System;
using System.Collections.Generic;
using Bazaar.Common.Posts;
using MongoDB.Bson;

namespace Bazaar.MongoDbStorage.Posts
{
    public class MongoDbPostPhotos : IPostPhotos
    {
        public ObjectId _id { get; set; }
        public ObjectId ProfileId { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? DeletedAt { get; set; }
        public List<string> ImagePaths { get; set; }
    }
}