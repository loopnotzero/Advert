using System;
using System.Net;
using System.Threading.Tasks;
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
            try
            {
                ViewData["returnUrl"] = returnUrl;
                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            try
            {
                ViewData["returnUrl"] = returnUrl;
                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SignOut(string returnUrl = null)
        {
            try
            {
                ViewData["returnUrl"] = returnUrl;
                await _signInManager.SignOutAsync();
                return Ok();
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
                await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                return Ok();
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

                await _userManager.CreateAsync(user, model.Password);

                var profile = new MongoDbProfile
                {
                    Name = model.Name,
                    CreatedAt = DateTime.UtcNow
                };

                await _profilesManager.CreateAsync(profile);
                
                await _signInManager.SignInAsync(user, true);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }
    }
}