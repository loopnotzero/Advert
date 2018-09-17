using System;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Egghead.Managers
{
    public class ProfilesManager<T> : IDisposable where T : class
    {
        public void Dispose()
        {
        }

        public Task<T> GetProfileByIdAsync(ObjectId userProfileId)
        {
            throw new NotImplementedException();
        }
    }
}