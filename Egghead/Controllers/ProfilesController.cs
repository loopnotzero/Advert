using System;
using System.IO;
using System.Threading.Tasks;
using Egghead.Managers;
using Egghead.Models.Profiles;
using Egghead.MongoDbStorage.Profiles;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.OData.Query.SemanticAst;
using Microsoft.Extensions.Logging;

namespace Egghead.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        private readonly ProfilesImagesManager<MongoDbProfileImage> _profilesImagesManager;

        public ProfilesController(ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer, IHostingEnvironment hostingEnvironment, ProfilesManager<MongoDbProfile> profilesManager, ProfilesImagesManager<MongoDbProfileImage> profilesImagesManager)
        {
            _logger = loggerFactory.CreateLogger<ProfilesController>();
            _hostingEnvironment = hostingEnvironment;
            
            _profilesManager = profilesManager;
            _profilesImagesManager = profilesImagesManager;
        }

        [HttpGet]
        [Route("/Profile")]
        public async Task<IActionResult> Profile()
        {
            var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

            if (profile == null)
            {
                //todo: Add user not found page
                return NotFound();
            }
            
            return View(new ProfileModel
            {
                Id = profile.Id.ToString(),
                Name = profile.Name,
                ImagePath = profile.ImagePath,
                ArticlesCount = ((double)0).ToMetric(),
                FollowingCount = ((double)0).ToMetric()
            });
        }
        
        [HttpPost("AddImage")]
        [Route("/Profile/AddImage")]
        public async Task<IActionResult> AddImage(string returnUrl, IFormFile file)
        {
            ViewData["ReturnUrl"] = returnUrl;
                 
            if (file.Length <= 0)
            {
                //todo: return View with ModelErrors
                return BadRequest();
            }
            
            var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

            if (profile == null)
            {
                //todo: Add user not found page
                return NotFound();
            }

            var dirPath = Path.Combine(_hostingEnvironment.WebRootPath + "/images/profiles/", profile.Id.ToString());

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var imagePath = Path.Combine(dirPath, file.FileName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            await _profilesImagesManager.CreateProfileImageAsync(new MongoDbProfileImage
            {
                ImagePath = imagePath,
                ProfileId = profile.Id,
                CreatedAt = DateTime.UtcNow
            });

            profile.ImagePath = Path.Combine(dirPath, file.FileName);

            await _profilesManager.UpdateProfileAsync(profile);

            return View("Profile", new ProfileModel
            {
                Id = profile.Id.ToString(),
                Name = profile.Name,
                ImagePath = profile.ImagePath,
                ArticlesCount = ((double)0).ToMetric(),
                FollowingCount = ((double)0).ToMetric()
            });
        }
    }
}