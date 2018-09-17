using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Managers;
using Egghead.Models.Errors;
using Egghead.Models.Users;
using Egghead.MongoDbStorage.Profiles;
using Egghead.MongoDbStorage.Users;
using Egghead.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Egghead.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly UserManager<MongoDbUser> _userManager;
        private readonly SignInManager<MongoDbUser> _signInManager;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        
        public AccountController(
            UserManager<MongoDbUser> userManager,
            SignInManager<MongoDbUser> signInManager,
            ProfilesManager<MongoDbProfile> profilesManager,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
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
        public IActionResult Register(string returnUrl = null)
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
                await _signInManager.SignOutAsync();

                return Ok(new ErrorModel
                {
                    RedirectUrl = returnUrl,
                    ErrorStatusCode = ErrorStatusCode.TemporaryRedirect,
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
            try
            {
                ViewData["returnUrl"] = returnUrl;
                
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

                if (!result.Succeeded)
                {
                    throw new Exception("Invalid email or password");
                }
                
                return Ok(new ErrorModel
                {
                    RedirectUrl = returnUrl,
                    ErrorStatusCode = ErrorStatusCode.TemporaryRedirect
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
        public async Task<IActionResult> Register([FromBody] RegisterModel model, string returnUrl = null)
        {
            try
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
                    throw new Exception("Couldn't create user: Description");         
                }

                _profilesManager.CreateProfile(new MongoDbProfile
                {
                    FirstName = "",
                    LastName = "",
                    Headline = "",
                    Location = "",
                    PhoneNumber = "",                    
                    CreatedAt = DateTime.UtcNow
                });
                
                await _signInManager.SignInAsync(user, false);

                return Ok(new ErrorModel
                {
                    RedirectUrl = returnUrl,
                    ErrorStatusCode = ErrorStatusCode.TemporaryRedirect
                });      
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }
    }
}