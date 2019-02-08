using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Bazaar.Common.Posts
{
    public interface IPostPhotos
    {
        ObjectId _id { get; set; }
        ObjectId ProfileId { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        DateTime? DeletedAt { get; set; }
        List<string> ImagePaths { get; set; }
    }
}