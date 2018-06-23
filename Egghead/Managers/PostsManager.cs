using System;

namespace Egghead.Managers
{
    public class PostsManager<T> : IDisposable where T : class
    {
        public void Dispose()
        {
        }
    }
}