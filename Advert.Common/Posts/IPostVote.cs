using System;
using Advert.Common.Posts;
using MongoDB.Bson;

namespace Advert.Common.Posts
{
    public interface IPostVote
    {
        string Email { get; set; }
        string NormalizedEmail { get; set; }
        ObjectId Id { get; set; }
        ObjectId PostId { get; set; }
        VoteType VoteType { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        DateTime DeletedAt { get; set; }
    }
}