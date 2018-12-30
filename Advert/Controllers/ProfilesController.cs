using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Advert.Managers;
using Advert.Models.Profiles;
using Advert.MongoDbStorage.AdsTopics;
using Advert.MongoDbStorage.Profiles;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Advert.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        private readonly ProfilesImagesManager<MongoDbProfileImage> _profilesImagesManager;
        private readonly AdsTopicsManager<MongoDbAdsTopic> _adsTopicsManager;
        private readonly AdsTopicCommentsManager<MongoDbAdsTopicComment> _adsTopicCommentsManager;

        public ProfilesController(ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer,
            IHostingEnvironment hostingEnvironment, ProfilesManager<MongoDbProfile> profilesManager,
            ProfilesImagesManager<MongoDbProfileImage> profilesImagesManager,
            AdsTopicsManager<MongoDbAdsTopic> adsTopicsManager,
            AdsTopicCommentsManager<MongoDbAdsTopicComment> adsTopicCommentsManager)
        {
            _logger = loggerFactory.CreateLogger<ProfilesController>();
            _hostingEnvironment = hostingEnvironment;
            _profilesManager = profilesManager;
            _profilesImagesManager = profilesImagesManager;
            _adsTopicsManager = adsTopicsManager;
            _adsTopicCommentsManager = adsTopicCommentsManager;
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
                AdsTopicsCount = ((double) 0).ToMetric(),
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
                AdsTopicsCount = ((double) 0).ToMetric(),
            });
        }

        [HttpPost("AddImage")]
        [Route("/Profile/AddImage")]
        public async Task<IActionResult> AddImage(string returnUrl, IFormFile file)
        {
            try
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

                foreach (var adsTopic in await _adsTopicsManager.FindAdsTopicsAsync(null))
                {
                    var adsTopicComments =
                        await _adsTopicCommentsManager.FindAdsTopicCommentsByProfileIdAsync(adsTopic.Id.ToString(),
                            adsTopic.ProfileId);

                    foreach (var adsTopicComment in adsTopicComments)
                    {
                        // ReSharper disable once InvertIf
                        if (adsTopicComment.ProfileId.Equals(profile.Id))
                        {                   
                            if (adsTopicComment.ProfileImagePath == null || !adsTopicComment.Equals(profile.ImagePath))
                            {
                                adsTopicComment.ProfileImagePath = profile.ImagePath;
                                await _adsTopicCommentsManager.UpdateAdsTopicCommentByIdAsync(adsTopicComment.AdsId.ToString(), adsTopicComment.Id, adsTopicComment);
                            }
                        }
                    }
                }

                foreach (var adsTopic in await _adsTopicsManager.FindAdsTopicsByProfileIdAsync(profile.Id))
                {
                    if (adsTopic.ProfileImagePath == null || !adsTopic.ProfileImagePath.Equals(profile.ImagePath))
                    {
                        adsTopic.ProfileImagePath = profile.ImagePath;
                        await _adsTopicsManager.UpdateAdsTopicAsync(adsTopic);
                    }
                }

                return RedirectToAction("GetAdsTopics", "AdsTopics");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }
    }
}