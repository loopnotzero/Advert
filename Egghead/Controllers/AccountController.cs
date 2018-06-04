using System.Threading.Tasks;
using Egghead.Models;
using Egghead.MongoDbStorage.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Egghead.Controllers
{
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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;

            _logger.LogInformation($"Model: {model}");

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorViewModel
                {
                    Message = "Invalid model"
                });
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: true);
                    
            if (!result.Succeeded)
            {
                return Unauthorized();
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

            var user = new MongoDbIdentityUser(model.Email);

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
    }
}