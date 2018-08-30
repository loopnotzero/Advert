using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Egghead.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger _logger;

        public ProfileController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        [HttpGet]
        public IActionResult GetProfile()
        {
            throw new NotImplementedException();
        }
    }
}