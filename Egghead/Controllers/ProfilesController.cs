using System;
using System.IO;
using System.Threading.Tasks;
using Egghead.Managers;
using Egghead.Models.Profiles;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Profiles;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Remotion.Linq.Clauses.ResultOperators;

namespace Egghead.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        private readonly ProfilesImagesManager<MongoDbProfileImage> _profilesImagesManager;
        private readonly ArticlesManager<MongoDbArticle> _articlesManager;
        private readonly ArticlesCommentsManager<MongoDbArticleComment> _articlesCommentsManager;

        public ProfilesController(ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer, IHostingEnvironment hostingEnvironment, ProfilesManager<MongoDbProfile> profilesManager, ProfilesImagesManager<MongoDbProfileImage> profilesImagesManager, ArticlesManager<MongoDbArticle> articlesManager, ArticlesCommentsManager<MongoDbArticleComment> articlesCommentsManager)
        {
            _logger = loggerFactory.CreateLogger<ProfilesController>();
            _hostingEnvironment = hostingEnvironment;         
            _profilesManager = profilesManager;
            _profilesImagesManager = profilesImagesManager;
            _articlesManager = articlesManager;
            _articlesCommentsManager = articlesCommentsManager;
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
                Image = profile.ImagePath,
                ArticlesCount = ((double)0).ToMetric(),
                FollowingCount = ((double)0).ToMetric()
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
                Image = profile.ImagePath,
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

            var articlesCount = await _articlesManager.EstimatedArticlesCountAsync();

            var articles = await _articlesManager.FindArticlesAsync((int) articlesCount);

            foreach (var article in articles)
            {
                var commentsCount = await _articlesCommentsManager.CountArticleCommentsByArticleIdAsync(article.Id.ToString());

                var articleComments = await _articlesCommentsManager.FindArticleCommentsByCollectionName(article.Id.ToString(), (int) commentsCount);

                foreach (var articleComment in articleComments)
                {
                    if (profile.Id.Equals(article.ProfileId) && !profile.ImagePath.Equals(articleComment.ProfileImage))
                    {
                        articleComment.ProfileImage = profile.ImagePath;
                        await _articlesCommentsManager.UpdateArticleCommentByIdAsync(article.Id.ToString(), articleComment.Id, articleComment);
                    }
                }
            }


            return RedirectToAction("Articles", "Articles");
        }
    }
}