using System.Collections.Generic;
using System.Threading.Tasks;
using Egghead.Managers;
using Egghead.MongoDbStorage.Profiles;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Egghead.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ILogger _logger;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ProfilesImagesManager<MongoDbProfileImage> _profilesImagesManager;

        public ProfilesController(
            ILoggerFactory loggerFactory,
            ILookupNormalizer keyNormalizer,
            IHostingEnvironment hostingEnvironment,
            ProfilesImagesManager<MongoDbProfileImage> profilesImagesManager
        )
        {
            _logger = loggerFactory.CreateLogger<ProfilesController>();
            _keyNormalizer = keyNormalizer;
            _hostingEnvironment = hostingEnvironment;
            _profilesImagesManager = profilesImagesManager;
        }

        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }
        
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(List<IFormFile> files)
        {
            _logger.LogInformation($"Web root path: {_hostingEnvironment.WebRootPath}");
            _logger.LogInformation($"Content root path: {_hostingEnvironment.ContentRootPath}");
            
//            var filePath = "/egghead/profiles/images/";
//
//            var file = files.First();
//
//            if (file.Length <= 0) return Ok();
//            
//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok();
        }
    }
}