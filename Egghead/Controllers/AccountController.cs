using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Managers;
using Egghead.Models.Captcha;
using Egghead.Models.Users;
using Egghead.MongoDbStorage.Profiles;
using Egghead.MongoDbStorage.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Egghead.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly UserManager<MongoDbUser> _userManager;
        private readonly SignInManager<MongoDbUser> _signInManager;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        
        public AccountController(ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer, IConfiguration configuration, UserManager<MongoDbUser> userManager, SignInManager<MongoDbUser> signInManager, ProfilesManager<MongoDbProfile> profilesManager)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _keyNormalizer = keyNormalizer;
            _configuration = configuration;     
            
            _userManager = userManager;
            _signInManager = signInManager;
            _profilesManager = profilesManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LogIn(string returnUrl = null)
        {
//            ViewData["SiteKey"] = _configuration.GetSection("GoogleReCaptchaOptions").GetValue<string>("SiteKey");
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
        public async Task<IActionResult> LogIn(LogInModel model, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

//            var form = HttpContext.Request.Form["g-recaptcha-response"];
//
//            var captchaResult = await GetCaptchaValidationResultAsync(form,
//                _configuration.GetSection("GoogleReCaptchaOptions").GetValue<string>("SecretKey"), _logger);
//
//            if (!captchaResult.Success)
//            {
//                _logger.LogError(captchaResult.ErrorCodes[0]);
//                ModelState.AddModelError(ModelErrorKey.ReCaptchaErrorStringPropertyName, captchaResult.ErrorCodes[0]);
//                return View(model);
//            }

            await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;

            var user = new MongoDbUser
            {
                Email = model.Email,
                NormalizedEmail = NormalizeKey(model.Email),
                UserName = model.Email,
                NormalizedUserName = NormalizeKey(model.Email),
            };

            await _userManager.CreateAsync(user, model.Password);
            
            await _signInManager.SignInAsync(user, false);

            await _profilesManager.CreateProfileAsync(new MongoDbProfile
            {
                Name = model.Name,
                NormalizedName = NormalizeKey(model.Name),
                NormalizedEmail = NormalizeKey(model.Email),
                CreatedAt = DateTime.UtcNow
            });
                              
            return Ok(new
            {
                returnUrl
            });
        }
        
        
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(ArticlesController.Articles), "Articles");
        }
        
        private string NormalizeKey(string key)
        {
            return _keyNormalizer != null ? _keyNormalizer.Normalize(key) : key;
        }

        private static async Task<CaptchaResult> GetCaptchaValidationResultAsync(string gRecaptchaResponse, string secret, ILogger logger)
        {
            var httpClient = new HttpClient();
            
            var response = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={gRecaptchaResponse}").Result;
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                logger.LogError("Error while sending request to ReCaptcha");
                //todo: throw captcha exception
                return null;
            }

            var captchaResult = await response.Content.ReadAsAsync<CaptchaResult>();
           
            return captchaResult;
        }
    }
}