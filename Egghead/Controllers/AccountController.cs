using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Models;
using Egghead.MongoDbStorage.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Egghead.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly UserManager<MongoDbIdentityUser> _userManager;     
        private readonly SignInManager<MongoDbIdentityUser> _signInManager;
        
        public AccountController(
            UserManager<MongoDbIdentityUser> userManager, 
            SignInManager<MongoDbIdentityUser> signInManager, 
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            _logger.LogInformation("ReturnUrl: {0}", returnUrl);
            return View();
        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            _logger.LogInformation("ReturnUrl: {0}", returnUrl);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PasswordReset(string returnUrl = null)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;

            _logger.LogInformation($"Model: {model}");

            if (!ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    return BadRequest(new ErrorViewModel
                    {
                        ErrorMessage = "Please enter your email",
                        ResponseStatusCode = ResponseStatusCode.EmailValidationFailed
                    }); 
                }
                
                if (string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest(new ErrorViewModel
                    {
                        ErrorMessage = "Please enter your password",
                        ResponseStatusCode = ResponseStatusCode.PasswordValidationFailed
                    }); 
                }       
            }

            if (await _userManager.FindByEmailAsync(model.Email) == null)
            {
                return Ok(new ErrorViewModel
                {
                    ErrorMessage = "This email does not seem to exist",
                    ResponseStatusCode = ResponseStatusCode.EmailNotFound
                });
            }

            if (await _userManager.CheckPasswordAsync(new MongoDbIdentityUser(model.Email, model.Password), model.Password))
            {
                return Ok(new ErrorViewModel
                {
                    ErrorMessage = "Wrong password. Try again or reset it",
                    ResponseStatusCode = ResponseStatusCode.PasswordDoNotMatch
                });
            }
            
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorViewModel
                {
                    ErrorMessage = "Authorization failed. Please contact your administrator",
                    ResponseStatusCode = ResponseStatusCode.AuthorizationFailed
                });
            }
            
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            _logger.LogInformation("ReturnUrl: {0}", returnUrl);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("RegisterViewModel state is not valid");
                return View(model);
            }

            var user = new MongoDbIdentityUser(model.Email, model.Password);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);             
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordResetViewModel model, string returnUrl)
        {
            throw new NotImplementedException();
        }
    }
}