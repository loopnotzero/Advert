using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bazaar.Common.Profiles;
using Bazaar.Models.Post;
using Bazaar.Models.Profiles;
using Bazaar.Models.Settings;
using Bazaar.MongoDbStorage.Posts;
using Bazaar.MongoDbStorage.Profiles;
using Bazaar.Services;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace Bazaar.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly PostsService<MongoDbPost> _postsService;
        private readonly ProfilesService<MongoDbProfile> _profilesService;
        private readonly PostsVotesService<MongoDbPostVote> _postsVotesService;
        private readonly PostCommentsService<MongoDbPostComment> _postCommentsService;

        public ProfilesController(
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment,
            PostsService<MongoDbPost> postsService,
            ProfilesService<MongoDbProfile> profilesService, 
            PostsVotesService<MongoDbPostVote> postsVotesService,
            PostCommentsService<MongoDbPostComment> postCommentsService 
        )
        {
            _logger = loggerFactory.CreateLogger<ProfilesController>();
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _postsService = postsService;
            _profilesService = profilesService;
            _postsVotesService = postsVotesService;
            _postCommentsService = postCommentsService;
        }

        [HttpGet]
        [Route("/{profileName}")]
        public async Task<IActionResult> GetProfile(string profileName)
        {
            try
            {
                return await GetProfilePosts(profileName);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("/{profileName}/Posts")]
        public async Task<IActionResult> GetProfilePosts(string profileName)
        {
            try
            {
                var profile = await _profilesService.FindProfileByNormalizedNameOrDefaultAsync(profileName, null);

                if (profile == null)
                {
                    return BadRequest();
                }

                var posts = await _postsService.FindPostsByIdentityNameAsync(profile.IdentityName, 0, 100);

                return await GetPosts(profile, posts);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/{profileName}/Hidden")]
        public async Task<IActionResult> GetProfileHiddenPosts()
        {
            try
            {
                var profile = await _profilesService.FindProfileByIdentityName(HttpContext.User.Identity.Name);

                var posts = await _postsService.FindHiddenPostsByIdentityNameAsync(profile.IdentityName, 0, 100);

                return await GetPosts(profile, posts);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/{profileName}/Favorites")]
        public async Task<IActionResult> GetProfileFavoritesPosts(string profileName)
        {
            try
            {
                IProfile profile =
                    await _profilesService.FindProfileByIdentityName(HttpContext.User.Identity.Name);

                var postsVotes = await _postsVotesService.FindPostsVotesOwnedByAsync(profile.IdentityName);

                var posts = postsVotes.Count > 0
                    ? await _postsService.FindPostsAsync(postsVotes.Select(x => x.PostId).ToList())
                    : new List<MongoDbPost>();

                return await GetPosts(profile, posts);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/Profile/AddProfilePhotoAsync")]
        public async Task<IActionResult> AddProfilePhotoAsync(IFormFile file)
        {
            try
            {
                var profile = await _profilesService.FindProfileByIdentityName(HttpContext.User.Identity.Name);
                
                var photoDir = $"profiles/{profile._id}";

                var photoSystemDir = $"{_hostingEnvironment.WebRootPath}/{photoDir}";

                if (!Directory.Exists(photoSystemDir))
                {
                    Directory.CreateDirectory(photoSystemDir);
                }

                using (var imageStream = file.OpenReadStream())
                {
                    var format = Image.DetectFormat(imageStream);

                    if (format.Equals(PngFormat.Instance))
                    {
                        var pngImage = Image.Load(imageStream);
                    
                        using (var stream = new FileStream($"{photoSystemDir}/profile__photo.jpg", FileMode.Create))
                        {
                            pngImage.SaveAsJpeg(stream);
                        }
                    }
                }

                return Ok(new AddProfilePhotoResult
                {
                    ProfileName = profile.Name,
                    ProfilePhoto = $"{photoDir}/profile__photo.jpg"
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }          
        }

        [HttpDelete]
        [Authorize]
        [Route("/Profile/RemoveProfilePhotoAsync")]
        public async Task<IActionResult> RemoveProfilePhotoAsync()
        {
            try
            {
                var profile = await _profilesService.FindProfileByIdentityName(HttpContext.User.Identity.Name);
                
                var photoDir = $"{_hostingEnvironment.WebRootPath}/profiles/{profile._id}";

                if (Directory.Exists(photoDir))
                {
                    Directory.Delete(photoDir);
                }

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }
        
        [HttpPut]
        [Authorize]
        [Route("/Profile/UpdateProfileByIdAsync")]
        public async Task<IActionResult> UpdateProfileByIdAsync([FromQuery(Name = "profileId")] string profileId,
            [FromBody] ProfileModel model)
        {
            try
            {
                var profile = await _profilesService.FindProfileByIdAsync(ObjectId.Parse(profileId));

                if (model.Gender != null)
                {
                    if (Enum.TryParse(model.Gender.Trim(), out Gender gender))
                    {
                        profile.Gender = gender;
                    }
                }

                profile.Location = model.Location;
                
                if (model.Birthday != null)
                {
                    profile.Birthday = DateTime.ParseExact(model.Birthday, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                
                profile.CallingCode = model.CallingCode;

                profile.PhoneNumber = model.PhoneNumber;

                var result = await _profilesService.UpdateProfileAsync(profile);

                return Ok(new UpdateResultModel
                {
                    MatchedCount = result.MatchedCount,
                    ModifiedCount = result.ModifiedCount,
                    IsAcknowledged = result.IsAcknowledged,
                    IsModifiedCountAvailable = result.IsModifiedCountAvailable
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }  
        
        private async Task<IActionResult> GetPosts(IProfile profile, IEnumerable<MongoDbPost> posts)
        {
            var countryCodes = _configuration.GetSection("CountryCodes").Get<List<CountryCode>>();

            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return View(new PostsAggregatorModel
                {
                    Posts = posts.Select(post => new PostModel
                    {
                        Hidden = post.Hidden,
                        PostId = post._id.ToString(),
                        Text = post.Text.Length > 1000 ? post.Text.Substring(0, 1000) + "..." : post.Text,
                        Title = post.Title,
                        Currency = post.Currency,
                        Location = post.Location,
                        CreatedAt = post.CreatedAt.Humanize(),
                        ViewsCount = ((double) post.ViewsCount).ToMetric(),
                        LikesCount = ((double) post.LikesCount).ToMetric(),
                        SharesCount = ((double) 0).ToMetric(),
                        CommentsCount = ((double) post.CommentsCount).ToMetric(),
                        ProfilePhoto = post.ProfilePhoto,
                        Price = post.Price,
                        Tags = post.Tags,
                    }),

                    Profile = new ProfileModel
                    {
                        Owner = false,
                        Id = profile._id.ToString(),
                        Name = profile.Name,
                        Email = profile.Email,
                        Gender = profile.Gender.ToString(),
                        Location = profile.Location,
                        Birthday = profile.Birthday?.ToString("dd MMMM yyyy"),
                        CreatedAt = profile.CreatedAt.Humanize(),
                        ImagePath = profile.Photo,
                        CallingCode = profile.CallingCode,
                        PhoneNumber = profile.PhoneNumber,
                        CountryCodes = countryCodes.Select(x => new CountryCode
                        {
                            CountryName = x.CountryName,
                            CallingCode = x.CallingCode
                        })
                    },

                    PlacesApi = _configuration.GetSection("GoogleApiServices").GetValue<string>("PlacesApi"),
                });
            }

            var myProfile = await _profilesService.FindProfileByIdentityNameOrDefaultAsync(HttpContext.User.Identity.Name,null);

            List<MongoDbPostVote> postsVotes = null;

            if (myProfile != null)
            {
                postsVotes = await _postsVotesService.FindPostsVotesOwnedByAsync(myProfile.IdentityName);
            }

            return View(new PostsAggregatorModel
            {
                Posts = posts.Select(post => new PostModel
                {
                    Hidden = post.Hidden,
                    IsOwner = myProfile != null && post.IdentityName.Equals(profile.IdentityName),
                    IsVoted = postsVotes != null && postsVotes.Any(x => x.PostId.Equals(post._id)),
                    PostId = post._id.ToString(),
                    Text = post.Text.Length > 1000 ? post.Text.Substring(0, 1000) + "..." : post.Text,
                    Title = post.Title,
                    Currency = post.Currency,
                    Location = post.Location,
                    CreatedAt = post.CreatedAt.Humanize(),
                    ViewsCount = ((double) post.ViewsCount).ToMetric(),
                    LikesCount = ((double) post.LikesCount).ToMetric(),
                    SharesCount = ((double) 0).ToMetric(),
                    CommentsCount = ((double) post.CommentsCount).ToMetric(),
                    ProfileName = post.ProfileName,
                    ProfilePhoto = post.ProfilePhoto,
                    Price = post.Price,
                    Tags = post.Tags,
                }),

                Profile = new ProfileModel
                {
                    Owner = myProfile != null && profile._id.Equals(myProfile._id),
                    Id = profile._id.ToString(),
                    Name = profile.Name,
                    Email = profile.Email,
                    Gender = profile.Gender.Humanize(),
                    Location = profile.Location,
                    Birthday = profile.Birthday?.ToString("dd MMMM yyyy"),
                    CreatedAt = profile.CreatedAt.Humanize(),
                    ImagePath = profile.Photo,
                    CallingCode = profile.CallingCode,
                    PhoneNumber = profile.PhoneNumber,
                    CountryCodes = countryCodes.Select(x => new CountryCode
                    {
                        CountryName = x.CountryName,
                        CallingCode = x.CallingCode
                    })
                },

                PlacesApi = _configuration.GetSection("GoogleApiServices").GetValue<string>("PlacesApi"),
            });
        }
    }
}