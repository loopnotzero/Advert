using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Advert.Common.Profiles;
using Advert.Managers;
using Advert.Models.Post;
using Advert.Models.Profiles;
using Advert.MongoDbStorage.Posts;
using Advert.MongoDbStorage.Profiles;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Advert.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly PostsManager<MongoDbPost> _postsManager;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        private readonly PostsVotesManager<MongoDbPostVote> _postsVotesManager;
        private readonly ProfilesImagesManager<MongoDbProfileImage> _profilesImagesManager;

        private const string EmptyProfileImage = "/images/no-image.png";

        public ProfilesController(
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment,
            PostsManager<MongoDbPost> postsManager,
            ProfilesManager<MongoDbProfile> profilesManager,
            PostsVotesManager<MongoDbPostVote> postsVotesManager,
            ProfilesImagesManager<MongoDbProfileImage> profilesImagesManager)
        {
            _logger = loggerFactory.CreateLogger<ProfilesController>();
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _postsManager = postsManager;
            _profilesManager = profilesManager;
            _postsVotesManager = postsVotesManager;
            _profilesImagesManager = profilesImagesManager;
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
                 
                var posts = await _postsManager.FindPostsByProfileIdAsync(profile._id, 0, null);

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
                            ProfileImagePath = EmptyProfileImage,
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
                            PhoneNumber = profile.PhoneNumber,
                        },
                        
                        PlacesApi = _configuration.GetSection("GoogleApiServices").GetValue<string>("PlacesApi"),
                    });
                }

                IProfile myProfile = null;

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    myProfile = await _profilesManager.FindProfileByNormalizedEmailOrDefaultAsync(HttpContext.User.Identity.Name, null);
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
                        IsOwner = myProfile != null && post.ProfileId.Equals(profile._id) && profile._id.Equals(myProfile._id),
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
                        ProfileImagePath = post.ProfileImagePath ?? EmptyProfileImage,
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
                        PhoneNumber = profile.PhoneNumber
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
                
                var posts = await _postsManager.FindHiddenPostsByProfileIdAsync(profile._id, 0, null);

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
                            ProfileImagePath = EmptyProfileImage,
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
                            PhoneNumber = profile.PhoneNumber
                        },
                        
                        PlacesApi = _configuration.GetSection("GoogleApiServices").GetValue<string>("PlacesApi"),  
                    });
                }

                IProfile myProfile = null;

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    myProfile = await _profilesManager.FindProfileByNormalizedEmailOrDefaultAsync(HttpContext.User.Identity.Name, null);
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
                        IsOwner = myProfile != null && post.ProfileId.Equals(profile._id) && profile._id.Equals(myProfile._id),
                        IsVoted = postsVotes != null && postsVotes.Count > 0 && postsVotes.Any(x => x.PostId.Equals(post._id)),
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
                        ProfileImagePath = post.ProfileImagePath ?? EmptyProfileImage,
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
                        PhoneNumber = profile.PhoneNumber
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
        [Route("/{profileName}/Favorites")]
        public async Task<IActionResult> GetProfileFavoritesPosts(string profileName)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return Unauthorized();
                }
                
                IProfile profile = await _profilesManager.FindProfileByNormalizedNameAsync(profileName);

                if (profile == null)
                {
                    return BadRequest();
                }
                
                var postsVotes = await _postsVotesManager.FindPostsVotesAsync(profile._id);

                var posts = postsVotes.Count > 0 ? await _postsManager.FindPostsAsync(postsVotes.Select(x => x.PostId).ToList()) : null;

                IProfile myProfile = null;

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    myProfile = await _profilesManager.FindProfileByNormalizedEmailOrDefaultAsync(HttpContext.User.Identity.Name, null);
                }
                     
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
                        ProfileImagePath = post.ProfileImagePath ?? EmptyProfileImage,
                        Price = post.Price,
                        Tags = post.Tags,
                    }),
                    
                    Profile = new ProfileViewModel
                    {
                        Owner = myProfile != null && profile.NormalizedEmail.Equals(HttpContext.User.Identity.Name.ToUpper()),
                        Id = profile._id.ToString(),
                        Name = profile.Name,
                        Email = profile.Email,
                        Gender = profile.Gender.Humanize(),
                        Location = profile.Location,
                        Birthday = profile.Birthday?.ToString("dd MMMM yyyy"),
                        CreatedAt = profile.CreatedAt.Humanize(),
                        ImagePath = profile.ImagePath,
                        PhoneNumber = profile.PhoneNumber
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
        
        [HttpPut]
        [Authorize]
        [Route("/Profile/UpdateProfileByIdAsync")]
        public async Task<IActionResult> UpdateProfileByIdAsync([FromQuery(Name = "profileId")] string profileId, [FromBody] ProfileViewModel model)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByIdAsync(ObjectId.Parse(profileId));

                if (Enum.TryParse(model.Gender, out Gender gender))
                {
                    profile.Gender = gender;
                }

                profile.Location = model.Location;

                if (string.IsNullOrEmpty(model.Birthday) || string.IsNullOrWhiteSpace(model.Birthday))
                {
                    profile.Birthday = null;
                }
                else
                {
                    profile.Birthday = DateTime.ParseExact(model.Birthday, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }

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