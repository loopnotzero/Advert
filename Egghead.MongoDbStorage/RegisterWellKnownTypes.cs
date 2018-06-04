using System;
using System.Threading;
using Egghead.MongoDbStorage.Identity;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Egghead.MongoDbStorage
{
    public class RegisterWellKnownTypes
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
            BsonClassMap.RegisterClassMap<MongoDbIdentityUser>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id).SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);
                cm.MapCreator(user => new MongoDbIdentityUser(user.Email));
            });
        }
    }
}