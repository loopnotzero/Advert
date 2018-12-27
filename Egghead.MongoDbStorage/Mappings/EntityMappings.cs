using System.Threading;
using Egghead.MongoDbStorage.Advertisements;
using Egghead.MongoDbStorage.Profiles;
using Egghead.MongoDbStorage.Users;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Egghead.MongoDbStorage.Mappings
{
    public static class EntityMappings
    {
        private static bool _initialized;
        private static object _initializationLock = new object();
        private static object _initializationTarget;   
        
        public static void EnsureMongoDbUserConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbUser();
                return null;
            });
        }
        
        public static void EnsureMongoDbProfileConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbProfile();
                return null;
            });
        }

        public static void EnsureMongoDbAdvertisementConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbAdvertisement();
                return null;
            });
        }

        public static void EnsureMongoDbProfileImageConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbProfileImage();
                return null;
            });
        }

        public static void EnsureMongoDbAdvertismentVoteConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbAdvertisementVote();
                return null;
            });
        }

        public static void EnsureMongoDbAdvertisementViewsConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbAdvertisementViews();
                return null;
            });
        }

        public static void EnsureMongoDbAdvertisementCommentConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbAdvertisementComment();
                return null;
            });
        }

        public static void EnsureMongoDbAdvertisementCommentVoteConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbAdvertisementCommentVote();
                return null;
            });
        }
        
        public static void EnsureMongoDbAdvertisementCommentsVotesAggregationConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbAdvertisementCommentsVotesAggregation();
                return null;
            });
        }

        private static void ConfigureMongoDbUser()
        {
            BsonClassMap.RegisterClassMap<MongoDbUser>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(c => c.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(user => new MongoDbUser(user.Email, user.NormalizedEmail, user.UserName, user.NormalizedUserName));
            });
        }
       
        private static void ConfigureMongoDbAdvertisement()
        {
            BsonClassMap.RegisterClassMap<MongoDbAdvertisement>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(advertisement => advertisement.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(advertisement => new MongoDbAdvertisement());
            });
        }
        
        private static void ConfigureMongoDbProfile()
        {
            BsonClassMap.RegisterClassMap<MongoDbProfile>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(profile => profile.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(profile => new MongoDbProfile());
            });
        }
        
        private static void ConfigureMongoDbAdvertisementVote()
        {
            BsonClassMap.RegisterClassMap<MongoDbAdvertisementVote>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(advertisementVote => advertisementVote.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(advertisementVote => new MongoDbAdvertisementVote());
            });
        }

        private static void ConfigureMongoDbProfileImage()
        {
            BsonClassMap.RegisterClassMap<MongoDbProfileImage>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(profileImage => profileImage.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(profileImage => new MongoDbProfileImage());
            });
        }

        private static void ConfigureMongoDbAdvertisementViews()
        {
            BsonClassMap.RegisterClassMap<MongoDbAdvertisementViewsCount>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(advertisementViewsCount => advertisementViewsCount.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(advertisementViewsCount => new MongoDbAdvertisementViewsCount());
            });
        }

        private static void ConfigureMongoDbAdvertisementComment()
        {
            BsonClassMap.RegisterClassMap<MongoDbAdvertisementComment>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(advertisementComment => advertisementComment.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(advertisementComment => new MongoDbAdvertisementComment());
            });
        }

        private static void ConfigureMongoDbAdvertisementCommentVote()
        {
            BsonClassMap.RegisterClassMap<MongoDbAdvertisementCommentVote>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(advertisementCommentVote => advertisementCommentVote.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(advertisementCommentVote => new MongoDbAdvertisementCommentVote());
            });
        }
        
        private static void ConfigureMongoDbAdvertisementCommentsVotesAggregation()
        {
            BsonClassMap.RegisterClassMap<MongoDbAdvertisementCommentVoteAggregation>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(advertisementCommentVoteAggregation => advertisementCommentVoteAggregation.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(advertisementCommentVoteAggregation => new MongoDbAdvertisementCommentVoteAggregation());
            });
        }
    }
}