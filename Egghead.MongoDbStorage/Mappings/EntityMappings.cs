using System.Threading;
using Egghead.MongoDbStorage.Entities;
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

        private static void ConfigureMongoDbUser()
        {
            BsonClassMap.RegisterClassMap<MongoDbUser>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(c => c.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(user => new MongoDbUser(user.UserName, user.Email));
            });
        }

        private static void ConfigureMongoDbArticle()
        {
            BsonClassMap.RegisterClassMap<MongoDbArticle>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(c => c.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(user => new MongoDbArticle());
            });
        }

        private static void ConfigureMongoDbArticleLike()
        {
            BsonClassMap.RegisterClassMap<MongoDbArticleLike>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(c => c.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(user => new MongoDbArticleLike());
            });
        }

        private static void ConfigureMongoDbArticleViews()
        {
            BsonClassMap.RegisterClassMap<MongoDbArticleViewCount>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(c => c.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(user => new MongoDbArticleViewCount());
            });
        }

        private static void ConfigureMongoDbArticleComment()
        {
            BsonClassMap.RegisterClassMap<MongoDbArticleComment>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(c => c.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(user => new MongoDbArticleComment());
            });
        }
    }
}