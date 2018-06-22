using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Models;
using Egghead.MongoDbStorage.Entities;
using Egghead.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Egghead.Controllers
{
    //todo: Added string constants to config file or as consts

    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly UserManager<MongoDbUser> _userManager;
        private readonly SignInManager<MongoDbUser> _signInManager;

        public AccountController(UserManager<MongoDbUser> userManager, SignInManager<MongoDbUser> signInManager, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _userManager = userManager;
            _signInManager = signInManager;                                 
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LogIn(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            _logger.LogInformation("ReturnUrl: {0}", returnUrl);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignUp(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            _logger.LogInformation("ReturnUrl: {0}", returnUrl);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            _logger.LogInformation("ReturnUrl: {0}", returnUrl);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn([FromBody] LoginViewModel model, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;

            _logger.LogInformation($"Model: {model}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is not valid: " + ModelState);
                //todo: Highlight all input forms
                return Ok(new ErrorViewModel
                {
                    TagName = "email",
                    ErrorMessage = "Enter your email",
                    ErrorStatusCode = ErrorStatusCode.UnprocessableEntity
                });
            }

            if (!AccountValidation.IsEmailValid(model.Email))
            {
                return Ok(new ErrorViewModel
                {
                    TagName = "email",
                    ErrorMessage = "Enter your email",
                    ErrorStatusCode = ErrorStatusCode.UnprocessableEntity
                });
            }

            if (!AccountValidation.IsPasswordValid(model.Password))
            {
                return Ok(new ErrorViewModel
                {
                    TagName = "password",
                    ErrorMessage = "Enter your password",
                    ErrorStatusCode = ErrorStatusCode.UnprocessableEntity
                });
            }

            var identityUser = await _userManager.FindByEmailAsync(model.Email);

            if (identityUser == null)
            {
                return Ok(new ErrorViewModel
                {
                    TagName = "email",
                    ErrorMessage = "Couldn't find your Egghead account",
                    ErrorStatusCode = ErrorStatusCode.Unauthorized
                });
            }

            if (!await _userManager.CheckPasswordAsync(identityUser, model.Password))
            {
                return Ok(new ErrorViewModel
                {
                    TagName = "password",
                    ErrorMessage = "Wrong password. Try again or click forgot password to reset it",
                    ErrorStatusCode = ErrorStatusCode.Unauthorized
                });
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return StatusCode((int) HttpStatusCode.OK, new ErrorViewModel
                {
                    //todo: TagName
                    ErrorMessage = "An error occured",
                    ErrorStatusCode = ErrorStatusCode.InternalServerError
                });
            }

            return Ok(new ErrorViewModel
            {
                RedirectUrl = returnUrl,
                ErrorStatusCode = ErrorStatusCode.Found
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([FromBody] SignUpViewModel model, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is not valid: " + ModelState);
                //todo: Highlight all input forms
                return Ok(new ErrorViewModel
                {
                    TagName = "email",
                    ErrorMessage = "Enter your email",
                    ErrorStatusCode = ErrorStatusCode.UnprocessableEntity
                });
            }

            if (!AccountValidation.IsEmailValid(model.Email))
            {
                _logger.LogWarning("Email is null or not valid");
                return Ok(new ErrorViewModel
                {
                    TagName = "email",
                    ErrorMessage = "Enter your email",
                    ErrorStatusCode = ErrorStatusCode.UnprocessableEntity
                });
            }

            if (!AccountValidation.IsFisrtNameValid(model.FirstName))
            {
                _logger.LogWarning("First Name is null or not valid");
                return Ok(new ErrorViewModel
                {
                    TagName = "firstName",
                    ErrorMessage = "Enter your first name",
                    ErrorStatusCode = ErrorStatusCode.UnprocessableEntity
                });
            }

            if (!AccountValidation.IsLastNameValid(model.LastName))
            {
                _logger.LogWarning("Last Name is null or not valid");
                return Ok(new ErrorViewModel
                {
                    TagName = "lastName",
                    ErrorMessage = "Enter your last name",
                    ErrorStatusCode = ErrorStatusCode.UnprocessableEntity
                });
            }

            if (!AccountValidation.IsPasswordValid(model.Password))
            {
                _logger.LogWarning("Password is null or not valid");
                return Ok(new ErrorViewModel
                {
                    TagName = "password",
                    ErrorMessage = "Enter your password",
                    ErrorStatusCode = ErrorStatusCode.UnprocessableEntity
                });
            }

            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                _logger.LogWarning("Email already exists");
                return Ok(new ErrorViewModel
                {
                    TagName = "email",
                    ErrorMessage = "That email is taken. Try another one",
                    ErrorStatusCode = ErrorStatusCode.Conflict
                });
            }

            var identityUser = new MongoDbUser
            {
                Email = model.Email,
                NormalizedEmail = model.Email,
                UserName = model.Email,
                NormalizedUserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName             
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (!result.Succeeded)
            {
                _logger.LogError(string.Join(", ", result.Errors.Select(x => $"Error Code: {x.Code} Description: {x.Description}")));
                return Ok(new ErrorViewModel
                {
                    TagName = "error",
                    ErrorMessage = "An error occurred",
                    ErrorStatusCode = ErrorStatusCode.InternalServerError
                });
            }

            //todo: Email confirmation

            await _signInManager.SignInAsync(identityUser, isPersistent: false);
            _logger.LogInformation("User authenticated: " + model);

            return Ok(new ErrorViewModel
            {
                RedirectUrl = returnUrl,
                ErrorStatusCode = ErrorStatusCode.Found
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] PasswordResetViewModel model, string returnUrl)
        {
            throw new NotImplementedException();
        }
    }
}