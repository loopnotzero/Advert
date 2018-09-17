using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Egghead.Managers;
using Egghead.MongoDbStorage.Profiles;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Egghead.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ProfileImageManager<MongoDbProfileImage> _profileImageManager;

        public ProfilesController(IHostingEnvironment hostingEnvironment, ProfileImageManager<MongoDbProfileImage> profileImageManager, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ProfilesController>();
            _hostingEnvironment = hostingEnvironment;
            _profileImageManager = profileImageManager;
        }

        [HttpGet]
        public IActionResult GetProfile()
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