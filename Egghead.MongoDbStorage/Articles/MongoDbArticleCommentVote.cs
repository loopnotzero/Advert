using System;
using Egghead.Common.Articles;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Articles
{
    public class MongoDbArticleCommentVote
    {
        public MongoDbArticleCommentVote()
        {
            Id = ObjectId.GenerateNewId().ToString();
            //Create indeces
        }

        public string Id { get; set; }
        public string ByWho { get; set; }
        public string ByWhoNormalized { get; set; }
        public string ArticleId { get; set; }
        public string CommentId { get; set; }
        public VoteType VoteType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}