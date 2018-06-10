using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.MongoDbStorage.Identities;
using Egghead.Utils;
using Egghead.ViewModels;
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

            if (!ModelState.IsValid) return View(model);

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

            var passwordHash = _userManager.PasswordHasher.HashPassword(identityUser, model.Password);

            if (!await _userManager.CheckPasswordAsync(identityUser, passwordHash))
            {
                return Ok(new ErrorViewModel
                {
                    TagName = "password",
                    ErrorMessage = "Wrong password. Try again or click forgot password to reset it",
                    ErrorStatusCode = ErrorStatusCode.Unauthorized
                });
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, passwordHash, isPersistent: true, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return StatusCode((int) HttpStatusCode.OK, new ErrorViewModel
                {
                    //todo: TagName
                    ErrorMessage = "An error occured",
                    ErrorStatusCode = ErrorStatusCode.InternalServerError
                });
            }

            return RedirectToLocal(returnUrl);
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
                return View(model);
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

            var identityUser = new MongoDbIdentityUser(model.Email);

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (!result.Succeeded)
            {
                _logger.LogError("Couldn't create user: " + string.Join(", ", result.Errors.Select(x => $"Code: {x.Code} Description: {x.Description}")));
                return Ok(new ErrorViewModel
                {
                    //todo: Handle TagName on client side
                    ErrorMessage = "An error occurred",
                    ErrorStatusCode = ErrorStatusCode.InternalServerError
                });
            }

            //todo: Email confirmation

            await _signInManager.SignInAsync(identityUser, isPersistent: false);
            _logger.LogInformation("User authenticated: " + model);

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] PasswordResetViewModel model, string returnUrl)
        {
            throw new NotImplementedException();
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}