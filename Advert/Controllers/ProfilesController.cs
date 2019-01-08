using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Advert.Managers;
using Advert.Models.Profiles;
using Advert.MongoDbStorage.Posts;
using Advert.MongoDbStorage.Profiles;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Advert.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        private readonly ProfilesImagesManager<MongoDbProfileImage> _profilesImagesManager;
        private readonly PostsManager<MongoDbPost> _postsManager;
        private readonly PostCommentsManager<MongoDbPostComment> _postCommentsManager;

        public ProfilesController(ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer,
            IHostingEnvironment hostingEnvironment, ProfilesManager<MongoDbProfile> profilesManager,
            ProfilesImagesManager<MongoDbProfileImage> profilesImagesManager,
            PostsManager<MongoDbPost> postsManager,
            PostCommentsManager<MongoDbPostComment> postCommentsManager)
        {
            _logger = loggerFactory.CreateLogger<ProfilesController>();
            _hostingEnvironment = hostingEnvironment;
            _profilesManager = profilesManager;
            _profilesImagesManager = profilesImagesManager;
            _postsManager = postsManager;
            _postCommentsManager = postCommentsManager;
        }

        [HttpGet]
        [Route("/Profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                return View(new ProfileModel
                {
                    Id = profile.Id.ToString(),
                    ProfileName = profile.Name,
                    ImagePath = profile.ImagePath,
                    PostsCount = ((double) 0).ToMetric(),
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }  
        }
        
        [HttpGet]
        [Route("/Profiles/{profileId}")]
        public async Task<IActionResult> GetProfile(string profileId)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByIdAsync(ObjectId.Parse(profileId));

                return View(new ProfileModel
                {
                    Id = profile.Id.ToString(),
                    ProfileName = profile.Name,
                    ImagePath = profile.ImagePath,
                    PostsCount = ((double) 0).ToMetric(),
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }  
        }
        
        [HttpPost("AddImage")]
        [Route("/Profile/AddProfileImage")]
        public async Task<IActionResult> AddProfileImage(string returnUrl, IFormFile file)
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

                foreach (var post in await _postsManager.FindPostsAsync(null))
                {
                    var postComments =
                        await _postCommentsManager.FindPostCommentsByProfileIdAsync(post.Id.ToString(),
                            post.ProfileId);

                    foreach (var postComment in postComments)
                    {
                        // ReSharper disable once InvertIf
                        if (postComment.ProfileId.Equals(profile.Id))
                        {                   
                            if (postComment.ProfileImagePath == null || !postComment.Equals(profile.ImagePath))
                            {
                                postComment.ProfileImagePath = profile.ImagePath;
                                await _postCommentsManager.ReplacePostCommentAsync(postComment.PostId.ToString(), postComment.Id, postComment);
                            }
                        }
                    }
                }

                foreach (var post in await _postsManager.FindPostsByProfileIdAsync(profile.Id))
                {
                    if (post.ProfileImagePath == null || !post.ProfileImagePath.Equals(profile.ImagePath))
                    {
                        post.ProfileImagePath = profile.ImagePath;
                        await _postsManager.UpdatePostAsync(post);
                    }
                }

                return RedirectToAction("GetPosts", "Posts");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }             
    }
}