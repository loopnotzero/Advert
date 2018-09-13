using System;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Egghead.Common.Stores
{
    public interface IProfileImageStore<T> : IDisposable where T : class
    {
        Task<T> GetProfileImageByProfileIdAsync(ObjectId profileId);
    }
}