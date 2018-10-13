using System.Threading;
using Egghead.MongoDbStorage.Articles;
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
        
        public static void EnsureMongoDbArticleConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbArticle();
                return null;
            });
        }

        public static void EnsureMongoDbArticleLikeConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbArticleLike();
                return null;
            });
        }

        public static void EnsureMongoDbArticleViewsConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbArticleViews();
                return null;
            });
        }

        public static void EnsureMongoDbArticleCommentConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbArticleComment();
                return null;
            });
        }

        public static void EnsureMongoDbArticleCommentLikeConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbArticleCommentLike();
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
        
        public static void EnsureMongoDbProfileImageConfigured()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                ConfigureMongoDbProfileImage();
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
       
        private static void ConfigureMongoDbArticle()
        {
            BsonClassMap.RegisterClassMap<MongoDbArticle>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(article => article.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(article => new MongoDbArticle());
            });
        }

        private static void ConfigureMongoDbArticleLike()
        {
            BsonClassMap.RegisterClassMap<MongoDbArticleVote>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(articleVote => articleVote.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(articleVote => new MongoDbArticleVote());
            });
        }

        private static void ConfigureMongoDbArticleViews()
        {
            BsonClassMap.RegisterClassMap<MongoDbArticleViewsCount>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(articleViewCount => articleViewCount.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(articleViewCount => new MongoDbArticleViewsCount());
            });
        }

        private static void ConfigureMongoDbArticleComment()
        {
            BsonClassMap.RegisterClassMap<MongoDbArticleComment>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(articleComment => articleComment.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(articleComment => new MongoDbArticleComment());
            });
        }

        private static void ConfigureMongoDbArticleCommentLike()
        {
            BsonClassMap.RegisterClassMap<MongoDbArticleCommentVote>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(articleCommentVote => articleCommentVote.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(articleCommentVote => new MongoDbArticleCommentVote());
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
        
        private static void ConfigureMongoDbProfileImage()
        {
            BsonClassMap.RegisterClassMap<MongoDbProfilePhoto>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(profileImage => profileImage.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(profileImage => new MongoDbProfilePhoto());
            });
        }
    }
}