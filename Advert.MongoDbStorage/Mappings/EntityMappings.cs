using System.Threading;
using Advert.MongoDbStorage.Posts;
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

        public static void EnsureMongoDbPostConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbPost();
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

        public static void EnsureMongoDbPostVoteConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbPostVote();
                return null;
            });
        }

        public static void EnsureMongoDbPostViewsConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbPostViews();
                return null;
            });
        }

        public static void EnsureMongoDbPostCommentConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbPostComment();
                return null;
            });
        }

        public static void EnsureMongoDbPostCommentVoteConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbPostCommentVote();
                return null;
            });
        }
        
        public static void EnsureMongoDbPostCommentsVotesAggregationConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbPostCommentsVotesAggregation();
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
       
        private static void ConfigureMongoDbPost()
        {
            BsonClassMap.RegisterClassMap<MongoDbPost>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(post => post.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(post => new MongoDbPost());
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
        
        private static void ConfigureMongoDbPostVote()
        {
            BsonClassMap.RegisterClassMap<MongoDbPostVote>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(postVote => postVote.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(postVote => new MongoDbPostVote());
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

        private static void ConfigureMongoDbPostViews()
        {
            BsonClassMap.RegisterClassMap<MongoDbPostViewsCount>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(postViewsCount => postViewsCount.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(postViewsCount => new MongoDbPostViewsCount());
            });
        }

        private static void ConfigureMongoDbPostComment()
        {
            BsonClassMap.RegisterClassMap<MongoDbPostComment>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(postComment => postComment.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(postComment => new MongoDbPostComment());
            });
        }

        private static void ConfigureMongoDbPostCommentVote()
        {
            BsonClassMap.RegisterClassMap<MongoDbPostCommentVote>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(postCommentVote => postCommentVote.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(postCommentVote => new MongoDbPostCommentVote());
            });
        }
        
        private static void ConfigureMongoDbPostCommentsVotesAggregation()
        {
            BsonClassMap.RegisterClassMap<MongoDbPostCommentVoteAggregation>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(postCommentVoteAggregation => postCommentVoteAggregation.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(postCommentVoteAggregation => new MongoDbPostCommentVoteAggregation());
            });
        }
    }
}