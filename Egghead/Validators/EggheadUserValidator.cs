using System.Threading.Tasks;
using Egghead.MongoDbStorage.Identities;
using Microsoft.AspNetCore.Identity;

namespace Egghead.Validators
{
    public class EggheadUserValidator<T> : IUserValidator<T> where T : MongoDbIdentityUser
    {
        public Task<IdentityResult> ValidateAsync(UserManager<T> manager, T user)
        {
            //todo: Add identity user validation 
            return Task.FromResult(IdentityResult.Success);
        }
    }
}