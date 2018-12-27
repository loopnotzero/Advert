using System;
using System.IO;
using System.Threading.Tasks;
using Egghead.Managers;
using Egghead.Models.Profiles;
using Egghead.MongoDbStorage.Advertisements;
using Egghead.MongoDbStorage.Profiles;
using Humanizer;
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
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        private readonly ProfilesImagesManager<MongoDbProfileImage> _profilesImagesManager;
        private readonly AdvertisementsManager<MongoDbAdvertisement> _advertisementsManager;
        private readonly AdvertisementsCommentsManager<MongoDbAdvertisementComment> _advertisementsCommentsManager;

        public ProfilesController(ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer, IHostingEnvironment hostingEnvironment, ProfilesManager<MongoDbProfile> profilesManager, ProfilesImagesManager<MongoDbProfileImage> profilesImagesManager, AdvertisementsManager<MongoDbAdvertisement> advertisementsManager, AdvertisementsCommentsManager<MongoDbAdvertisementComment> advertisementsCommentsManager)
        {
            _logger = loggerFactory.CreateLogger<ProfilesController>();
            _hostingEnvironment = hostingEnvironment;         
            _profilesManager = profilesManager;
            _profilesImagesManager = profilesImagesManager;
            _advertisementsManager = advertisementsManager;
            _advertisementsCommentsManager = advertisementsCommentsManager;
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
                AdvertisementsCount = ((double)0).ToMetric(),
            });
        }
        
        [HttpGet]
        [Route("/Profile/{name}")]
        public async Task<IActionResult> Profile(string name)
        {
            var profile = await _profilesManager.FindProfileByNormalizedNameAsync(name);

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
                AdvertisementsCount = ((double)0).ToMetric(),
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

            var absoluteDir = $"{_hostingEnvironment.WebRootPath}/images/profiles/{profile.Id.ToString()}";

            if (Directory.Exists(absoluteDir))
            {
                Directory.Delete(absoluteDir, true);
            }
            
            Directory.CreateDirectory(absoluteDir);

            var absoluteImagePath = Path.Combine(absoluteDir, file.FileName);

            using (var stream = new FileStream(absoluteImagePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            await _profilesImagesManager.CreateProfileImageAsync(new MongoDbProfileImage
            {
                ProfileId = profile.Id,
                ImagePath = $"/images/profiles/{profile.Id}/{file.FileName}",
                CreatedAt = DateTime.UtcNow
            });

            profile.ImagePath = $"/images/profiles/{profile.Id}/{file.FileName}";

            await _profilesManager.UpdateProfileAsync(profile);
          
            //todo: Update optimization
            
            var advertisements = await _advertisementsManager.FindAdvertisementsAsync(null);

            foreach (var advertisement in advertisements)
            {
                var advertisementComments = await _advertisementsCommentsManager.FindAdvertisementCommentsByProfileIdAsync(advertisement.Id.ToString(), advertisement.ProfileId);
                foreach (var advertisementComment in advertisementComments)
                {              
                    // ReSharper disable once InvertIf
                    if (advertisementComment.ProfileId.Equals(profile.Id) && advertisementComment.ProfileImagePath != profile.ImagePath && !advertisement.ProfileImagePath.Equals(profile.ImagePath))
                    {
                        advertisementComment.ProfileImagePath = profile.ImagePath;
                        await _advertisementsCommentsManager.UpdateAdvertisementCommentByIdAsync(advertisementComment.AdsId.ToString(), advertisementComment.Id, advertisementComment);
                    }
                }
            }

            var dbAdvertisements = await _advertisementsManager.FindAdvertisementsByProfileIdAsync(profile.Id);
      
            foreach (var advertisement in dbAdvertisements)
            {
                advertisement.ProfileImagePath = profile.ImagePath;
                await _advertisementsManager.UpdateAdvertisementAsync(advertisement);
            }
            
            return RedirectToAction("GetAdvertisements", "Advertisements");
        }
    }
}