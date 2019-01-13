using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Advert.Common.Posts;
using Advert.Managers;
using Advert.Models.Post;
using Advert.Models.Profiles;
using Advert.MongoDbStorage.Posts;
using Advert.MongoDbStorage.Profiles;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Advert.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly PostsManager<MongoDbPost> _postsManager;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        private readonly PostsVotesManager<MongoDbPostVote> _postsVotesManager;
        private readonly ProfilesImagesManager<MongoDbProfileImage> _profilesImagesManager;

        private const string EmptyProfileImage = "/images/no-image.png";

        public ProfilesController(
            ILoggerFactory loggerFactory,
            IHostingEnvironment hostingEnvironment,
            PostsManager<MongoDbPost> postsManager,
            ProfilesManager<MongoDbProfile> profilesManager,
            PostsVotesManager<MongoDbPostVote> postsVotesManager,
            ProfilesImagesManager<MongoDbProfileImage> profilesImagesManager)
        {
            _logger = loggerFactory.CreateLogger<ProfilesController>();
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
                return await GetProfilePostsWithSellingItems(profileName);
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
                        Sold = post.Sold,
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
                    
                    Profile = new ProfileModel
                    {
                        Id = profile._id.ToString(),
                        Name = profile.Name,
                        Email = profile.Email,
                        ImagePath = profile.ImagePath,
                        PhoneNumber = profile.PhoneNumber
                    },
                    
                    IsProfileOwner = myProfile != null && profile.NormalizedEmail.Equals(HttpContext.User.Identity.Name.ToUpper())
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }
        
        [HttpGet]
        [Route("/{profileName}/Sold")]
        public async Task<IActionResult> GetProfilePostsWithSoldItems(string profileName)
        {
            try
            {
                IProfile profile = await _profilesManager.FindProfileByNormalizedNameAsync(profileName);

                if (profile == null)
                {
                    return BadRequest();
                }
                
                var posts = await _postsManager.FindPostsWithSoldItemsByProfileIdAsync(profile._id, 0, null);

                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return View(new PostsAggregatorViewModel
                    {
                        Posts = posts.Select(post => new PostViewModel
                        {
                            Sold = post.Sold,
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
                        
                        Profile = new ProfileModel
                        {
                            Id = profile._id.ToString(),
                            Name = profile.Name,
                            Email = profile.Email,
                            ImagePath = profile.ImagePath,
                            PhoneNumber = profile.PhoneNumber
                        },
                        
                        IsProfileOwner = false
                    });
                }

                IProfile myProfile = null;

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    myProfile = await _profilesManager.FindProfileByNormalizedEmailOrDefaultAsync(HttpContext.User.Identity.Name, null);
                }

                var postsVotes = await _postsVotesManager.FindPostsVotesAsync(myProfile._id);

                return View(new PostsAggregatorViewModel
                {
                    Posts = posts.Select(post => new PostViewModel
                    {
                        Sold = post.Sold,
                        IsOwner = post.ProfileId.Equals(profile._id) && profile._id.Equals(myProfile._id),
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
                    
                    Profile = new ProfileModel
                    {
                        Id = profile._id.ToString(),
                        Name = profile.Name,
                        Email = profile.Email,
                        ImagePath = profile.ImagePath,
                        PhoneNumber = profile.PhoneNumber
                    },
                    
                    IsProfileOwner = myProfile != null && profile._id.Equals(myProfile._id)
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("/{profileName}/Selling")]
        public async Task<IActionResult> GetProfilePostsWithSellingItems(string profileName)
        {
            try
            {
                IProfile profile = await _profilesManager.FindProfileByNormalizedNameAsync(profileName);

                if (profile == null)
                {
                    return BadRequest();
                }
                
                var posts = await _postsManager.FindPostsWithSellingItemsByProfileIdAsync(profile._id, 0, null);

                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return View(new PostsAggregatorViewModel
                    {
                        Posts = posts.Select(post => new PostViewModel
                        {
                            Sold = post.Sold,
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
                        
                        Profile = new ProfileModel
                        {
                            Id = profile._id.ToString(),
                            Name = profile.Name,
                            Email = profile.Email,
                            ImagePath = profile.ImagePath,
                            PhoneNumber = profile.PhoneNumber
                        },
                        
                        IsProfileOwner = false
                    });
                }

                IProfile myProfile = null;

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    myProfile = await _profilesManager.FindProfileByNormalizedEmailOrDefaultAsync(HttpContext.User.Identity.Name, null);
                }   

                var postsVotes = await _postsVotesManager.FindPostsVotesAsync(myProfile._id);

                return View(new PostsAggregatorViewModel
                {     
                    Posts = posts.Select(post => new PostViewModel
                    {
                        Sold = post.Sold,
                        IsOwner = post.ProfileId.Equals(profile._id) && profile._id.Equals(myProfile._id),
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
                    
                    Profile = new ProfileModel
                    {
                        Id = profile._id.ToString(),
                        Name = profile.Name,
                        Email = profile.Email,
                        ImagePath = profile.ImagePath,
                        PhoneNumber = profile.PhoneNumber
                    },
                    
                    IsProfileOwner = myProfile != null && profile._id.Equals(myProfile._id),
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