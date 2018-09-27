using System;
using System.Collections.Generic;
using Egghead.Common.Profiles;
using MongoDB.Bson;

namespace Egghead.MongoDbStorage.Profiles
{
    public class MongoDbProfile
    {
        public MongoDbProfile()
        {
            Id = ObjectId.GenerateNewId();
            //Create indeces
        }

        public ObjectId Id { get; set; }           
        public string Name { get; set; }    
        public string NormalizedName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public string Location { get; set; }
        public string LocationNormalized { get; set; }             
        public string PhoneNumber { get; set; }            
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }  
        public DateTime DeletedAt { get; set; }     
    }
}