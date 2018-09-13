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
        public string FirstName { get; set; }
        public string NormalizedFirstName { get; set; }
        public string LastName { get; set; }
        public string NormalizedLastName { get; set; }
        public string Headline { get; set; }
        public string HeadlineNormalized { get; set; }
        public string Location { get; set; }
        public string LocationNormalized { get; set; }
        public string PhoneNumber { get; set; }
        public MongoDbBirthday Birthday { get; set; }
        public List<SocialLink> SocialLinks { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }       
    }
}