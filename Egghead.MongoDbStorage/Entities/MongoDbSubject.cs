using System;
using Egghead.Common;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Entities
{
    public class MongoDbSubject
    {
        public MongoDbSubject()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string NormalizedTitle { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
        public DateTime RemovedAt { get; set; }
        public ReleaseType ReleaseType { get; set; }
    }
}