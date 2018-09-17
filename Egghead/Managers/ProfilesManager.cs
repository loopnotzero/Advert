using System;
using Egghead.MongoDbStorage.Profiles;

namespace Egghead.Managers
{
    public class ProfilesManager<T> : IDisposable where T : class
    {
        public void Dispose()
        {
        }

        public void CreateProfile(MongoDbProfile profile)
        {
            
        }
    }
}