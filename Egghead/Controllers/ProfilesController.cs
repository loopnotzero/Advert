using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Egghead.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ILogger _logger;

        public ProfilesController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        [HttpGet]
        public IActionResult GetProfile()
        {
            return View();
        }
    }
}