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
    //todo: Added string constants to config file or as consts
    
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly UserManager<MongoDbIdentityUser> _userManager;
        private readonly SignInManager<MongoDbIdentityUser> _signInManager;

        public AccountController(UserManager<MongoDbIdentityUser> userManager, SignInManager<MongoDbIdentityUser> signInManager, ILoggerFactory loggerFactory)
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
                IActionResult badRequest = null;
                if (!IsCredentialsValidationError(model.Email, model.Password, ref badRequest))
                {
                    return badRequest;
                }
            }

            if (await _userManager.FindByEmailAsync(model.Email) == null)
            {
                return Ok(new ErrorViewModel
                {
                    ErrorMessage = "Couldn't find your Egghead account",
                    ResponseStatusCode = ResponseStatusCode.CouldNotFindYourEmail
                });
            }

            if (await _userManager.CheckPasswordAsync(new MongoDbIdentityUser(model.Email, model.Password), model.Password))
            {
                return Ok(new ErrorViewModel
                {
                    ErrorMessage = "Wrong password. Try again or click forgot password to reset it",
                    ResponseStatusCode = ResponseStatusCode.PasswordDidNotMatch
                });
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, new ErrorViewModel
                {
                    ErrorMessage = "Couldn't log in account. Please, contact technical support for help",
                    ResponseStatusCode = ResponseStatusCode.CouldNotAthorizeYourAccount
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

            _logger.LogInformation($"Model: {model}");

            if (!ModelState.IsValid)
            {
                IActionResult badRequest = null;
                if (!IsCredentialsValidationError(model.Email, model.Password, ref badRequest))
                {
                    return badRequest;
                }
            }

            if (await _userManager.FindByEmailAsync(model.Email) == null)
            {
                return Ok(new ErrorViewModel
                {
                    ErrorMessage = "That email is taken. Try another one",
                    ResponseStatusCode = ResponseStatusCode.ThatEmailIsTaken
                });
            }

            var result = await _userManager.CreateAsync(new MongoDbIdentityUser(model.Email, model.Password));

  
            if (!result.Succeeded)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, new ErrorViewModel
                {
                    ErrorMessage = "Couldn't register account. Please, contact technical support for assistance",
                    ResponseStatusCode = ResponseStatusCode.CouldNotRegisterYourAccount
                });
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordResetViewModel model, string returnUrl)
        {
            throw new NotImplementedException();
        }
        
        
        private bool IsCredentialsValidationError(string email, string password, ref IActionResult badRequest)
        {
            if (string.IsNullOrEmpty(email))
            {
                badRequest = BadRequest(new ErrorViewModel
                {
                    ErrorMessage = "Enter your email",
                    ResponseStatusCode = ResponseStatusCode.EmailValidationError
                });
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                badRequest = BadRequest(new ErrorViewModel
                {
                    ErrorMessage = "Enter your password",
                    ResponseStatusCode = ResponseStatusCode.PasswordValidationError
                });
                return false;
            }

            return true;
        }
    }
}