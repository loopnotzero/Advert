using System;
using MongoDB.Bson;

namespace Bazaar.Common.Profiles
{
    public interface IProfilePhoto
    {
        ObjectId _id { get; set; }
        ObjectId ProfileId { get; set; }  
        string ImagePath { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}