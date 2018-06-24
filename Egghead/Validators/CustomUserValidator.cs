using System.Threading.Tasks;
using Egghead.MongoDbStorage.Entities;
using Microsoft.AspNetCore.Identity;

namespace Egghead.Validators
{
    public class CustomUserValidator<T> : IUserValidator<T> where T : MongoDbUser
    {
        public Task<IdentityResult> ValidateAsync(UserManager<T> manager, T user)
        {
            //todo: Add identity user validation 
            return Task.FromResult(IdentityResult.Success);
        }
    }
}