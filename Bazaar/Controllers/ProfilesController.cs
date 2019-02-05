using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Advert.Managers;
using Bazaar.Common.Profiles;
using Bazaar.Managers;
using Bazaar.Models.Post;
using Bazaar.Models.Profiles;
using Bazaar.Models.Settings;
using Bazaar.MongoDbStorage.Posts;
using Bazaar.MongoDbStorage.Profiles;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Bazaar.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly PostsManager<MongoDbPost> _postsManager;
        private readonly PostCommentsManager<MongoDbPostComment> _postCommentsManager;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        private readonly PostsVotesManager<MongoDbPostVote> _postsVotesManager;
        private readonly ProfilesPhotosManager<MongoDbProfileImage> _profilesPhotosManager;

        public ProfilesController(
            ILoggerFactory loggerFactory, 
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment, 
            PostsManager<MongoDbPost> postsManager,
            ProfilesManager<MongoDbProfile> profilesManager, PostsVotesManager<MongoDbPostVote> postsVotesManager,
            PostCommentsManager<MongoDbPostComment> postCommentsManager,
            ProfilesPhotosManager<MongoDbProfileImage> profilesPhotosManager)
        {
            _logger = loggerFactory.CreateLogger<ProfilesController>();
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _postsManager = postsManager;
            _profilesManager = profilesManager;
            _postsVotesManager = postsVotesManager;
            _postCommentsManager = postCommentsManager;
            _profilesPhotosManager = profilesPhotosManager;
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
                IProfile profile = await _profilesManager.FindProfileByNormalizedNameAsync(profileName);

                if (profile == null)
                {
                    return BadRequest();
                }

                var posts = await _postsManager.FindPostsByProfileIdAsync(profile._id, 0, 100);

                var countryCodes = _configuration.GetSection("CountryCodes").Get<List<CountryCode>>();

                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return View(new PostsAggregatorViewModel
                    {
                        Posts = posts.Select(post => new PostViewModel
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
                            ProfileImagePath = post.ProfileImagePath,
                            Price = post.Price,
                            Tags = post.Tags,
                        }),

                        Profile = new ProfileViewModel
                        {
                            Owner = false,
                            Id = profile._id.ToString(),
                            Name = profile.Name,
                            Email = profile.Email,
                            Gender = profile.Gender.ToString(),
                            Location = profile.Location,
                            Birthday = profile.Birthday?.ToString("dd MMMM yyyy"),
                            CreatedAt = profile.CreatedAt.Humanize(),
                            ImagePath = profile.ImagePath,
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

                IProfile myProfile = null;

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    myProfile =
                        await _profilesManager.FindProfileByNormalizedEmailOrDefaultAsync(
                            HttpContext.User.Identity.Name, null);
                }

                List<MongoDbPostVote> postsVotes = null;

                if (myProfile != null)
                {
                    postsVotes = await _postsVotesManager.FindPostsVotesAsync(myProfile._id);
                }

                return View(new PostsAggregatorViewModel
                {
                    Posts = posts.Select(post => new PostViewModel
                    {
                        Hidden = post.Hidden,
                        IsOwner = myProfile != null && post.ProfileId.Equals(profile._id) &&
                                  profile._id.Equals(myProfile._id),
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
                        ProfileId = post.ProfileId.ToString(),
                        ProfileName = post.ProfileName,
                        ProfileImagePath = post.ProfileImagePath,
                        Price = post.Price,
                        Tags = post.Tags,
                    }),

                    Profile = new ProfileViewModel
                    {
                        Owner = myProfile != null && profile._id.Equals(myProfile._id),
                        Id = profile._id.ToString(),
                        Name = profile.Name,
                        Email = profile.Email,
                        Gender = profile.Gender.Humanize(),
                        Location = profile.Location,
                        Birthday = profile.Birthday?.ToString("dd MMMM yyyy"),
                        CreatedAt = profile.CreatedAt.Humanize(),
                        ImagePath = profile.ImagePath,
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
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("/{profileName}/Hidden")]
        public async Task<IActionResult> GetProfileHiddenPosts(string profileName)
        {
            try
            {
                IProfile profile = await _profilesManager.FindProfileByNormalizedNameAsync(profileName);

                if (profile == null)
                {
                    return BadRequest();
                }

                var posts = await _postsManager.FindHiddenPostsByProfileIdAsync(profile._id, 0, 100);

                var countryCodes = _configuration.GetSection("CountryCodes").Get<List<CountryCode>>();

                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return View(new PostsAggregatorViewModel
                    {
                        Posts = posts.Select(post => new PostViewModel
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
                            ProfileImagePath = post.ProfileImagePath,
                            Price = post.Price,
                            Tags = post.Tags,
                        }),

                        Profile = new ProfileViewModel
                        {
                            Owner = false,
                            Id = profile._id.ToString(),
                            Name = profile.Name,
                            Email = profile.Email,
                            Gender = profile.Gender.Humanize(),
                            Location = profile.Location,
                            Birthday = profile.Birthday?.ToString("dd MMMM yyyy"),
                            CreatedAt = profile.CreatedAt.Humanize(),
                            ImagePath = profile.ImagePath,
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

                IProfile myProfile = null;

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    myProfile =
                        await _profilesManager.FindProfileByNormalizedEmailOrDefaultAsync(
                            HttpContext.User.Identity.Name, null);
                }

                List<MongoDbPostVote> postsVotes = null;

                if (myProfile != null)
                {
                    postsVotes = await _postsVotesManager.FindPostsVotesAsync(myProfile._id);
                }

                return View(new PostsAggregatorViewModel
                {
                    Posts = posts.Select(post => new PostViewModel
                    {
                        Hidden = post.Hidden,
                        IsOwner = myProfile != null && post.ProfileId.Equals(profile._id) &&
                                  profile._id.Equals(myProfile._id),
                        IsVoted = postsVotes != null && postsVotes.Count > 0 &&
                                  postsVotes.Any(x => x.PostId.Equals(post._id)),
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
                        ProfileId = post.ProfileId.ToString(),
                        ProfileName = post.ProfileName,
                        ProfileImagePath = post.ProfileImagePath,
                        Price = post.Price,
                        Tags = post.Tags,
                    }),

                    Profile = new ProfileViewModel
                    {
                        Owner = myProfile != null && profile._id.Equals(myProfile._id),
                        Id = profile._id.ToString(),
                        Name = profile.Name,
                        Email = profile.Email,
                        Gender = profile.Gender.Humanize(),
                        Location = profile.Location,
                        Birthday = profile.Birthday?.ToString("dd MMMM yyyy"),
                        CreatedAt = profile.CreatedAt.Humanize(),
                        ImagePath = profile.ImagePath,
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
                IProfile profile = await _profilesManager.FindProfileByNormalizedNameAsync(profileName);

                if (profile == null)
                {
                    return BadRequest();
                }

                var postsVotes = await _postsVotesManager.FindPostsVotesAsync(profile._id);

                var posts = postsVotes.Count > 0
                    ? await _postsManager.FindPostsAsync(postsVotes.Select(x => x.PostId).ToList())
                    : null;

                IProfile myProfile = null;

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    myProfile =
                        await _profilesManager.FindProfileByNormalizedEmailOrDefaultAsync(
                            HttpContext.User.Identity.Name, null);
                }

                var countryCodes = _configuration.GetSection("CountryCodes").Get<List<CountryCode>>();

                return View(new PostsAggregatorViewModel
                {
                    Posts = posts?.Select(post => new PostViewModel
                    {
                        Hidden = post.Hidden,
                        IsOwner = post.ProfileId.Equals(profile._id),
                        IsVoted = postsVotes.Count > 0 && postsVotes.Any(x => x.PostId.Equals(post._id)),
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
                        ProfileId = post.ProfileId.ToString(),
                        ProfileName = post.ProfileName,
                        ProfileImagePath = post.ProfileImagePath,
                        Price = post.Price,
                        Tags = post.Tags,
                    }),

                    Profile = new ProfileViewModel
                    {
                        Owner = myProfile != null &&
                                profile.NormalizedEmail.Equals(HttpContext.User.Identity.Name.ToUpper()),
                        Id = profile._id.ToString(),
                        Name = profile.Name,
                        Email = profile.Email,
                        Gender = profile.Gender.Humanize(),
                        Location = profile.Location,
                        Birthday = profile.Birthday?.ToString("dd MMMM yyyy"),
                        CreatedAt = profile.CreatedAt.Humanize(),
                        ImagePath = profile.ImagePath,
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
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/Profile/AddProfilePhotoAsync")]
        public async Task<IActionResult> AddProfilePhotoAsync([FromQuery(Name = "profileId")] string profileId, IFormFile file)
        {
            var profile = await _profilesManager.FindProfileByIdAsync(ObjectId.Parse(profileId));

            if (!profile.Email.ToUpper().Equals(HttpContext.User.Identity.Name.ToUpper()))
            {
                return Ok(new ProfileErrorModel
                {
                    Errors = new List<ProfileError>
                    {
                        new ProfileError
                        {
                            Description = "You don't have permission to upload profile photo"
                        }
                    },
                });
            }

            var photoDir = $"{_hostingEnvironment.WebRootPath}/images/profiles/{profile._id.ToString()}/photo/";

            if (!Directory.Exists(photoDir))
            {
                Directory.CreateDirectory(photoDir);
            }

            using (var stream = new FileStream($"{photoDir}{Path.GetFileNameWithoutExtension(file.FileName)}.jpg", FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            var profileImage = new MongoDbProfileImage
            {
                ProfileId = profile._id,
                ImagePath = $"/images/profiles/{profile._id.ToString()}/photo/{Path.GetFileNameWithoutExtension(file.FileName)}.jpg",
                CreatedAt = DateTime.UtcNow
            };

            await _profilesPhotosManager.CreateProfileImageAsync(profileImage);

            profile.ImagePath = profileImage.ImagePath;

            await _profilesManager.UpdateProfileAsync(profile);

            return Ok(profileImage);
        }

        [HttpPost]
        [Authorize]
        [Route("/Profile/RemoveProfilePhotoAsync")]
        public async Task<IActionResult> RemoveProfilePhotoAsync([FromQuery(Name = "fileName")] string fileName,
            IFormFile file)
        {
            throw new NotImplementedException();
        }
        
        [HttpPut]
        [Authorize]
        [Route("/Profile/UpdateProfileByIdAsync")]
        public async Task<IActionResult> UpdateProfileByIdAsync([FromQuery(Name = "profileId")] string profileId,
            [FromBody] ProfileViewModel model)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByIdAsync(ObjectId.Parse(profileId));

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

                var result = await _profilesManager.UpdateProfileAsync(profile);

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
    }
}