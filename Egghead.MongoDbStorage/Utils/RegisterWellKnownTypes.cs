using System.Threading;
using Egghead.MongoDbStorage.Identities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Egghead.MongoDbStorage.Utils
{
    public static class RegisterWellKnownTypes
    {
        private static bool _initialized;
        private static object _initializationLock = new object();
        private static object _initializationTarget;

        public static void EnsureConfigure()
        {
            LazyInitializer.EnsureInitialized(ref _initializationTarget, ref _initialized, ref _initializationLock, () =>
            {
                Configure();
                return null;
            });
        }

        private static void Configure()
        {
            BsonClassMap.RegisterClassMap<MongoDbIdentityUser>(bsonClassMap =>
            {
                bsonClassMap.AutoMap();
                bsonClassMap.MapIdMember(c => c.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                bsonClassMap.MapCreator(user => new MongoDbIdentityUser(user.UserName, user.Email));
            });
        }
    }
}