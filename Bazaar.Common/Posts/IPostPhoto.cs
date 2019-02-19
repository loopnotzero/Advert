using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Bazaar.Common.Posts
{
    public interface IPostPhoto
    {
        ObjectId _id { get; set; }
        string PhotoPath { get; set; }
        string IdentityName { get; set; }
        ObjectId PostId { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}