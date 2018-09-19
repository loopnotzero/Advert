using System;
using System.Net;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Managers;
using Egghead.Models.Users;
using Egghead.MongoDbStorage.Profiles;
using Egghead.MongoDbStorage.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Egghead.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly UserManager<MongoDbUser> _userManager;
        private readonly SignInManager<MongoDbUser> _signInManager;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        
        public AccountController(ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer, UserManager<MongoDbUser> userManager, SignInManager<MongoDbUser> signInManager, ProfilesManager<MongoDbProfile> profilesManager)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _keyNormalizer = keyNormalizer;
            _userManager = userManager;
            _signInManager = signInManager;
            _profilesManager = profilesManager;
        }      

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LogIn(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignUp(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SignOut(string returnUrl = null)
        {
            try
            {
                ViewData["returnUrl"] = returnUrl;
                await _signInManager.SignOutAsync();
                return Ok(new
                {
                    returnUrl
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn([FromBody] LogInModel model, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl; 
            
            var signInResult = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (!signInResult.Succeeded)
            {
                //todo: Handle error
            }
                
            return Ok(new
            {
                returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([FromBody] RegisterModel model, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;

            var user = new MongoDbUser
            {
                Email = model.Email,
                NormalizedEmail = model.Email,
                UserName = model.Email,
                NormalizedUserName = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                //todo: Handle error            
            }

            await _signInManager.SignInAsync(user, false);
                              
            return Ok(new
            {
                returnUrl
            });
        }
    }
}