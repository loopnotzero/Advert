using System;
using MongoDB.Bson;

namespace Advert.Common.Profiles
{
    public interface IProfileImage
    {
        ObjectId Id { get; set; }
        ObjectId ProfileId { get; set; }  
        string ImagePath { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        DateTime DeletedAt { get; set; }
    }
}