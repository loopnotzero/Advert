using System.Threading;
using Advert.MongoDbStorage.AdsTopics;
using Advert.MongoDbStorage.Profiles;
using Advert.MongoDbStorage.Users;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Advert.MongoDbStorage.Mappings
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

        public static void EnsureMongoDbAdsTopicConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbAdsTopic();
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

        public static void EnsureMongoDbAdsTopicVoteConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbAdsTopicVote();
                return null;
            });
        }

        public static void EnsureMongoDbAdsTopicViewsConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbAdsTopicViews();
                return null;
            });
        }

        public static void EnsureMongoDbAdsTopicCommentConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbAdsTopicComment();
                return null;
            });
        }

        public static void EnsureMongoDbAdsTopicCommentVoteConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbAdsTopicCommentVote();
                return null;
            });
        }
        
        public static void EnsureMongoDbAdsTopicCommentsVotesAggregationConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbAdsTopicCommentsVotesAggregation();
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
       
        private static void ConfigureMongoDbAdsTopic()
        {
            BsonClassMap.RegisterClassMap<MongoDbAdsTopic>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(adsTopic => adsTopic.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(adsTopic => new MongoDbAdsTopic());
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
        
        private static void ConfigureMongoDbAdsTopicVote()
        {
            BsonClassMap.RegisterClassMap<MongoDbAdsTopicVote>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(adsTopicVote => adsTopicVote.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(adsTopicVote => new MongoDbAdsTopicVote());
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

        private static void ConfigureMongoDbAdsTopicViews()
        {
            BsonClassMap.RegisterClassMap<MongoDbAdsTopicViewsCount>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(adsTopicViewsCount => adsTopicViewsCount.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(adsTopicViewsCount => new MongoDbAdsTopicViewsCount());
            });
        }

        private static void ConfigureMongoDbAdsTopicComment()
        {
            BsonClassMap.RegisterClassMap<MongoDbAdsTopicComment>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(adsTopicComment => adsTopicComment.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(adsTopicComment => new MongoDbAdsTopicComment());
            });
        }

        private static void ConfigureMongoDbAdsTopicCommentVote()
        {
            BsonClassMap.RegisterClassMap<MongoDbAdsTopicCommentVote>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(adsTopicCommentVote => adsTopicCommentVote.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(adsTopicCommentVote => new MongoDbAdsTopicCommentVote());
            });
        }
        
        private static void ConfigureMongoDbAdsTopicCommentsVotesAggregation()
        {
            BsonClassMap.RegisterClassMap<MongoDbAdsTopicCommentVoteAggregation>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(adsTopicCommentVoteAggregation => adsTopicCommentVoteAggregation.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(adsTopicCommentVoteAggregation => new MongoDbAdsTopicCommentVoteAggregation());
            });
        }
    }
}