using System.Threading.Tasks;
using Bazaar.MongoDbStorage.Users;
using Microsoft.AspNetCore.Identity;

namespace Bazaar.Validators
{
    public class AdvertUserValidator<T> : IUserValidator<T> where T : MongoDbUser
    {
        public Task<IdentityResult> ValidateAsync(UserManager<T> manager, T user)
        {
            //todo: Add identity user validation 
            return Task.FromResult(IdentityResult.Success);
        }
    }
}