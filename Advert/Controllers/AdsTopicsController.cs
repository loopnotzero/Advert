using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Advert.Common.AdsTopics;
using Advert.Common.Metrics;
using Advert.Exceptions;
using Advert.Managers;
using Advert.Models.AdsTopic;
using Advert.Models.AdsTopics;
using Advert.Models.Profiles;
using Advert.MongoDbStorage.AdsTopics;
using Advert.MongoDbStorage.Profiles;
using Advert.MongoDbStorage.Users;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver.Core.Operations;

namespace Advert.Controllers
{
    public class AdsTopicsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        private readonly AdsTopicsManager<MongoDbAdsTopic> _adsTopicsManager;
        private readonly AdsTopicsVotesManager<MongoDbAdsTopicVote> _adsTopicsVotesManager;
        private readonly AdsTopicCommentsManager<MongoDbAdsTopicComment> _adsTopicCommentsManager;
        private readonly AdsTopicsViewsCountManager<MongoDbAdsTopicViewsCount> _adsTopicsViewsesCountManager;
        private readonly AdsTopicCommentsVotesManager<MongoDbAdsTopicCommentVote> _adsTopicCommentsVotesManager;
        private readonly AdsTopicCommentsVotesAggregationManager<MongoDbAdsTopicCommentVote, MongoDbAdsTopicCommentVoteAggregation> _adsTopicCommentsVotesAggregationManager;
        
        private const string NoProfileImage = "/images/no_image.png";

        public AdsTopicsController(ILoggerFactory loggerFactory,
            IConfiguration configuration,
            ILookupNormalizer keyNormalizer,
            IHostingEnvironment hostingEnvironment,
            UserManager<MongoDbUser> userManager,
            ProfilesManager<MongoDbProfile> profilesManager,
            AdsTopicsManager<MongoDbAdsTopic> adsTopicsManager,
            AdsTopicsVotesManager<MongoDbAdsTopicVote> adsTopicsVotesManager,
            AdsTopicCommentsManager<MongoDbAdsTopicComment> adsTopicCommentsManager,
            AdsTopicsViewsCountManager<MongoDbAdsTopicViewsCount> adsTopicsViewsesCountManager,
            AdsTopicCommentsVotesManager<MongoDbAdsTopicCommentVote> adsTopicCommentsVotesManager,
            AdsTopicCommentsVotesAggregationManager<MongoDbAdsTopicCommentVote, MongoDbAdsTopicCommentVoteAggregation> adsTopicCommentsVotesAggregationManager)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;         

            _profilesManager = profilesManager;
            _adsTopicsManager = adsTopicsManager;
            _adsTopicsVotesManager = adsTopicsVotesManager;
            _adsTopicCommentsManager = adsTopicCommentsManager;
            _adsTopicsViewsesCountManager = adsTopicsViewsesCountManager;
            _adsTopicCommentsVotesManager = adsTopicCommentsVotesManager;
            _adsTopicCommentsVotesAggregationManager = adsTopicCommentsVotesAggregationManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAdsTopics([FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "keyword")] string keyword = null)
        {
            try
            {
                var adsTopicsPerPage = _configuration.GetSection("AdvertOptions").GetValue<int>("AdsTopicsPerPage");                           
                
                var offset = (page - 1) * adsTopicsPerPage;
                                             
                List<MongoDbAdsTopic> adsTopics;

                if (string.IsNullOrEmpty(keyword))
                {
                    adsTopics = await _adsTopicsManager.FindAdsTopicsAsync(offset, adsTopicsPerPage);
                }
                else
                {
                    adsTopics = await _adsTopicsManager.FindAdsTopicsWhichContainsKeywordAsync(offset, adsTopicsPerPage, keyword);
                }

                var lastPage = (long) Math.Ceiling((double) await _adsTopicsManager.EstimatedAdsTopicsCountAsync() / adsTopicsPerPage);               
                
                var maxPages = _configuration.GetSection("AdvertOptions").GetValue<int>("MaxPages");
                
                var middlePosition = (long) Math.Ceiling((double) maxPages / 2);
                
                var beginPage = page - middlePosition;

                if (beginPage <= 0)
                {
                    beginPage = 1;
                }

                var endPage = page + middlePosition - 1;

                if (endPage > lastPage)
                {
                    endPage = lastPage;
                }
                else
                {
                    if (endPage < maxPages)
                    {
                        endPage = lastPage < maxPages ? lastPage : maxPages;
                    }
                }
                                             
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);
             
                return View(new AggregatorViewModel
                {
                    BeginPage = beginPage,
                    EndPage = endPage,
                    CurrentPage = page,
                    LastPage = lastPage,
                        
                    Profile = new ProfileModel
                    {
                        Id = profile.Id.ToString(),
                        Name = profile.Name,
                        ImagePath = profile.ImagePath ?? NoProfileImage,
                        AdsTopicsCount = ((double) await _adsTopicsManager.CountAdsTopicsByProfileIdAsync(profile.Id)).ToMetric(),
                    },

                    AdsTopics = adsTopics.Select(adsTopic => new AdsTopicViewModel
                    {
                        AdsId = adsTopic.Id.ToString(),
                        ProfileId = adsTopic.ProfileId.ToString(),
                        ProfileName = adsTopic.ProfileName,
                        ProfileImagePath = adsTopic.ProfileImagePath ?? NoProfileImage,
                        Text = adsTopic.Text.Length > 1000 ? adsTopic.Text.Substring(0, 1000) + "..." : adsTopic.Text,
                        Title = adsTopic.Title,
                        Price = adsTopic.Price.ToString(),
                        Location = adsTopic.Location,
                        Currency = adsTopic.Currency,
                        LikesCount = ((double) adsTopic.LikesCount).ToMetric(),
                        SharesCount = ((double) 0).ToMetric(),
                        ViewsCount = ((double) adsTopic.ViewsCount).ToMetric(),
                        CommentsCount = ((double) adsTopic.CommentsCount).ToMetric(),
                        CreatedAt = adsTopic.CreatedAt.Humanize(),
                        IsTopicOwner = adsTopic.ProfileId.Equals(profile.Id)
                    }),
                    
                    ApiService = new ApiServiceModel
                    {
                        ApiKey = _configuration.GetSection("GoogleOptions").GetValue<string>("ApiKey")
                    },

                    RecommendedAdsTopics = adsTopics
                        .OrderByDescending(x => EngagementRate.ComputeEngagementRate(x.LikesCount, x.SharesCount, x.CommentsCount, x.ViewsCount))
                        .Select(adsTopic => new RecommendedAdsTopicViewModel
                        {
                            AdsId = adsTopic.Id.ToString(),
                            ProfileName = adsTopic.ProfileName,
                            ProfileImagePath = adsTopic.ProfileImagePath ?? NoProfileImage,
                            Title = adsTopic.Title,
                            CreatedAt = adsTopic.CreatedAt.Humanize(),
                        }).ToList()                   
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
        [Route("/Topics/{adsId}")]
        public async Task<IActionResult> GetAdsTopicContent(string adsId, [FromQuery(Name = "page")] int page = 1)
        {
            try
            {
                var adsViewsCount = new MongoDbAdsTopicViewsCount
                {
                    Email = HttpContext.User.Identity.Name,
                    AdsId = ObjectId.Parse(adsId),
                    CreatedAt = DateTime.UtcNow
                };
                
                await _adsTopicsViewsesCountManager.CreateAdsTopicViewsCountAsync(adsViewsCount);
               
                var adsTopic = await _adsTopicsManager.FindAdsTopicByIdAsync(adsViewsCount.AdsId);
                
                adsTopic.ViewsCount = await _adsTopicsViewsesCountManager.CountAdsTopicViewsCountAsync(adsViewsCount.AdsId);
                
                await _adsTopicsManager.UpdateAdsTopicAsync(adsTopic);
       
                var adsTopicComments = await _adsTopicCommentsManager.FindAdsTopicCommentsAsync(adsId, 0, null, SortDefinition.Descending);
   
                var commentsReplies = new Dictionary<ObjectId, AdsTopicCommentViewModel>();

                foreach (var comments in adsTopicComments.OrderBy(x => x.ReplyTo).GroupBy(comment => comment.ReplyTo))
                {
                    if (comments.Key == ObjectId.Empty)
                    {
                        foreach (var comment in comments)
                        {
                            commentsReplies.Add(comment.Id, new AdsTopicCommentViewModel
                            {
                                Text = comment.Text,
                                ReplyTo = comment.ReplyTo.ToString(),
                                CommentId = comment.Id.ToString(),
                                AdsId = comment.AdsId.ToString(),
                                CreatedAt = comment.CreatedAt.Humanize(),
                                ProfileName = comment.ProfileName,
                                ProfileImagePath = comment.ProfileImagePath ?? NoProfileImage,
                                VotesCount = ((double) comment.VotesCount).ToMetric()
                            });
                        }
                    }
                    else
                    {
                        if (commentsReplies.TryGetValue(comments.Key, out var adsTopicComment))
                        {
                            if (adsTopicComment.Comments == null)
                            {
                                if (comments.Any())
                                {
                                    adsTopicComment.Comments = comments.OrderBy(x => x.CreatedAt).Select(comment =>
                                    {
                                        var model = new AdsTopicCommentViewModel
                                        {
                                            Text = comment.Text,
                                            ReplyTo = comment.ReplyTo.ToString(),
                                            CommentId = comment.Id.ToString(),
                                            AdsId = comment.AdsId.ToString(),
                                            CreatedAt = comment.CreatedAt.Humanize(),
                                            ProfileName = comment.ProfileName,
                                            ProfileImagePath = comment.ProfileImagePath ?? NoProfileImage,
                                            VotesCount = ((double) comment.VotesCount).ToMetric()
                                        };
                                        return model;
                                    }).ToList();
                                }
                            }
                            else
                            {
                                adsTopicComment.Comments.AddRange(comments.OrderBy(x => x.CreatedAt).Select(comment =>
                                {
                                    var model = new AdsTopicCommentViewModel
                                    {
                                        Text = comment.Text,
                                        ReplyTo = comment.ReplyTo.ToString(),
                                        CommentId = comment.Id.ToString(),
                                        AdsId = comment.AdsId.ToString(),
                                        CreatedAt = comment.CreatedAt.Humanize(),
                                        ProfileName = comment.ProfileName,
                                        ProfileImagePath = comment.ProfileImagePath ?? NoProfileImage,
                                        VotesCount = ((double) comment.VotesCount).ToMetric()
                                    };
                                    return model;
                                }));
                            }
                        }
                    }
                }

                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var adsTopics = await _adsTopicsManager.FindAdsTopicsAsync(_configuration.GetSection("AdvertOptions").GetValue<int>("HowManyAdsTopicsPerPage"));

                var orderedAdsTopics = adsTopics.OrderByDescending(x => EngagementRate.ComputeEngagementRate(x.LikesCount, x.SharesCount, x.CommentsCount, x.ViewsCount));

                return View(new AggregatorViewModel
                {
                    Profile = new ProfileModel
                    {
                        Name = profile.Name,
                        Id = profile.Id.ToString(),
                        ImagePath = profile.ImagePath ?? NoProfileImage,
                        AdsTopicsCount = ((double) await _adsTopicsManager.CountAdsTopicsByProfileIdAsync(adsTopic.ProfileId)).ToMetric(),                      
                    },                   
                    
                    AdsTopics = new List<AdsTopicViewModel>
                    {
                        new AdsTopicViewModel
                        {
                            AdsId = adsTopic.Id.ToString(),
                            ProfileId = adsTopic.ProfileId.ToString(),
                            ProfileName = adsTopic.ProfileName,
                            ProfileImagePath = adsTopic.ProfileImagePath ?? NoProfileImage,
                            Text = adsTopic.Text,
                            Title = adsTopic.Title,
                            Price = adsTopic.Price.ToString(),
                            Location = adsTopic.Location,
                            Currency = adsTopic.Currency,
                            LikesCount = ((double) adsTopic.LikesCount).ToMetric(),
                            SharesCount = ((double)0).ToMetric(),
                            ViewsCount = ((double) adsTopic.ViewsCount).ToMetric(),
                            CommentsCount = ((double) adsTopic.CommentsCount).ToMetric(),
                            CreatedAt = adsTopic.CreatedAt.Humanize()                           
                        }
                    }, 
                    
                    ApiService = new ApiServiceModel
                    {
                        ApiKey = _configuration.GetSection("GoogleOptions").GetValue<string>("ApiKey")
                    },
                    
                    RecommendedAdsTopics = orderedAdsTopics.Select(recommendedAdsTopic => new RecommendedAdsTopicViewModel
                    {
                        AdsId = recommendedAdsTopic.Id.ToString(),
                        ProfileName = recommendedAdsTopic.ProfileName,
                        ProfileImagePath = recommendedAdsTopic.ProfileImagePath ?? NoProfileImage,
                        Title = recommendedAdsTopic.Title,
                        CreatedAt = recommendedAdsTopic.CreatedAt.Humanize(),                      
                    }).ToList(),  
                    
                    AdsTopicComments = commentsReplies.Values.ToList(),
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
        [Route("/Topics/CreateAdsTopicAsync")]
        public async Task<IActionResult> CreateAdsTopicAsync([FromBody] CreateAdsTopicViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var adsTopic = new MongoDbAdsTopic
                {
                    Text = viewModel.Text,
                    Title = viewModel.Title,
                    Price = viewModel.Price,
                    Location = viewModel.Location,
                    Currency = viewModel.Currency,
                    ReleaseType = ReleaseType.PreModeration,
                    ProfileId = profile.Id,
                    ProfileName = profile.Name,
                    ProfileImagePath = profile.ImagePath,
                    CreatedAt = DateTime.UtcNow,
                };

                await _adsTopicsManager.CreateAdsTopicAsync(adsTopic);

                return Ok(new
                {
                    returnUrl = Url.Action("GetAdsTopicContent", "AdsTopics", new {adsId = adsTopic.Id})
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
        [Route("/Topics/UpdateAdsTopicByIdAsync/{adsId}")]
        public async Task<IActionResult> UpdateAdsTopicByIdAsync(string adsId, [FromBody] CreateAdsTopicViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var adsTopic = await _adsTopicsManager.FindAdsTopicByIdAsync(ObjectId.Parse(adsId));

                adsTopic.Text = viewModel.Text;
                adsTopic.Title = viewModel.Title;
                adsTopic.Price = viewModel.Price;
                adsTopic.Location = viewModel.Location;
                adsTopic.Currency = viewModel.Currency;

                await _adsTopicsManager.UpdateAdsTopicAsync(adsTopic);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/Topics/GetAdsTopicByIdAsync/{adsId}")]
        public async Task<IActionResult> GetAdsTopicByIdAsync(string adsId)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var adsTopic = await _adsTopicsManager.FindAdsTopicByIdAsync(ObjectId.Parse(adsId));
                
                return Ok(new AdsTopicViewModel
                {
                    AdsId = adsTopic.Id.ToString(),
                    ProfileName = adsTopic.ProfileName,
                    ProfileImagePath = adsTopic.ProfileImagePath ?? NoProfileImage,
                    Text = adsTopic.Text,
                    Title = adsTopic.Title,
                    Price = adsTopic.Price.ToString(),
                    Location = adsTopic.Location,
                    Currency = adsTopic.Currency,
                    LikesCount = ((double) adsTopic.LikesCount).ToMetric(),
                    SharesCount = ((double) 0).ToMetric(),
                    ViewsCount = ((double) adsTopic.ViewsCount).ToMetric(),
                    CommentsCount = ((double) adsTopic.CommentsCount).ToMetric(),
                    CreatedAt = adsTopic.CreatedAt.Humanize(),
                    IsTopicOwner = adsTopic.ProfileId.Equals(profile.Id)
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
        [Route("/Topics/DeleteAdsTopicByIdAsync/{adsId}")]
        public async Task<IActionResult> DeleteAdsTopicByIdAsync(string adsId)
        {
            try
            {
                await _adsTopicsManager.DeleteAdsTopicByIdAsync(ObjectId.Parse(adsId));
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/Topics/CreateAdsTopicVoteAsync")]
        public async Task<IActionResult> CreateAdsTopicVoteAsync([FromBody] AdsTopicVoteViewModel viewModel)
        {
            try
            {
                var adsId = ObjectId.Parse(viewModel.AdsId);

                var vote = await _adsTopicsVotesManager.FindAdsTopicVoteByNormalizedEmailAsync(adsId, HttpContext.User.Identity.Name);

                if (vote == null)
                {
                    await _adsTopicsVotesManager.CreateAdsTopicVoteAsync(new MongoDbAdsTopicVote
                    {
                        Email = HttpContext.User.Identity.Name,
                        VoteType = viewModel.VoteType,
                        AdsId = adsId,
                        CreatedAt = DateTime.UtcNow
                    });

                    var votesCount = await _adsTopicsVotesManager.CountAdsTopicVotesByVoteTypeAsync(adsId, viewModel.VoteType);

                    switch (viewModel.VoteType)
                    {
                        case VoteType.None:
                            var logString = $"Vote type is not valid. Ads id: {viewModel.AdsId} Email: {HttpContext.User.Identity.Name}";
                            throw new AdsTopicCommentVoteException(logString);
                        case VoteType.Like:
                            await _adsTopicsManager.UpdateAdsTopicLikesCountByAdsId(adsId, votesCount);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return Ok(new AdsTopicVotesViewModel
                    {
                        VoteType = viewModel.VoteType,
                        VotesCount = ((double) votesCount).ToMetric()
                    });
                }
                else
                {
                    if (vote.VoteType == viewModel.VoteType)
                    {
                        await _adsTopicsVotesManager.DeleteAdsTopicVoteByIdAsync(vote.Id);
                    }

                    var votesCount = await _adsTopicsVotesManager.CountAdsTopicVotesByVoteTypeAsync(adsId, viewModel.VoteType);

                    switch (viewModel.VoteType)
                    {
                        case VoteType.None:
                            var logString = $"Vote type is not valid. Ads id: {viewModel.AdsId} Email: {HttpContext.User.Identity.Name}";
                            throw new AdsTopicCommentVoteException(logString);
                        case VoteType.Like:
                            await _adsTopicsManager.UpdateAdsTopicLikesCountByAdsId(adsId, votesCount);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return Ok(new AdsTopicVotesViewModel
                    {
                        VoteType = viewModel.VoteType,
                        VotesCount = ((double) votesCount).ToMetric()
                    });
                }
            }
            catch (AdsTopicVoteException e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/Topics/CreateAdsTopicCommentAsync")]
        public async Task<IActionResult> CreateAdsTopicCommentAsync([FromBody] PublicCommentViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);
                
                var adsId = ObjectId.Parse(viewModel.AdsId);

                var adsTopicComment = new MongoDbAdsTopicComment
                {
                    Text = viewModel.Text,
                    ReplyTo = viewModel.ReplyTo == null ? ObjectId.Empty : ObjectId.Parse(viewModel.ReplyTo),
                    CreatedAt = DateTime.UtcNow,
                    AdsId = adsId,
                    ProfileId = profile.Id,
                    ProfileName = profile.Name,
                    ProfileImagePath = profile.ImagePath ?? NoProfileImage,
                    VotesCount = 0,
                };
              
                await _adsTopicCommentsManager.CreateAdsTopicComment(viewModel.AdsId, adsTopicComment);
                
                var comment = await _adsTopicCommentsManager.FindAdsTopicCommentById(viewModel.AdsId, adsTopicComment.Id);
                
                var adsTopic = await _adsTopicsManager.FindAdsTopicByIdAsync(comment.AdsId);
                
                adsTopic.CommentsCount = await _adsTopicCommentsManager.EstimatedAdsTopicCommentsByAdsIdAsync(viewModel.AdsId);
                
                await _adsTopicsManager.UpdateAdsTopicAsync(adsTopic);

                return Ok(new AdsTopicCommentViewModel
                {
                    Text = comment.Text,
                    ReplyTo = comment.ReplyTo == ObjectId.Empty ? null : comment.ReplyTo.ToString(),
                    CreatedAt = comment.CreatedAt.Humanize(),
                    CommentId = comment.Id.ToString(),
                    AdsId = viewModel.AdsId,
                    ProfileId = comment.ProfileId.ToString(),
                    ProfileName = comment.ProfileName,
                    ProfileImagePath = comment.ProfileImagePath ?? NoProfileImage,     
                    VotesCount = ((double)comment.VotesCount).ToMetric()
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
        [Route("/Topics/CreateAdsTopicCommentVoteAsync")]
        public async Task<IActionResult> CreateAdsTopicCommentVoteAsync([FromBody] AdsTopicCommentVoteViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var commentId = ObjectId.Parse(viewModel.CommentId);

                var commentVote = await _adsTopicCommentsVotesManager.FindAdsTopicCommentVoteOrDefaultAsync(commentId, profile.Id, null);

                if (commentVote != null)
                {
                    await _adsTopicCommentsVotesManager.DeleteAdsTopicCommentVoteByIdAsync(commentVote.Id);   
                }
                else
                {
                    await _adsTopicCommentsVotesManager.CreateAdsTopicCommentVoteAsync(new MongoDbAdsTopicCommentVote
                    {
                        AdsId = ObjectId.Parse(viewModel.AdsId),
                        VoteType = viewModel.VoteType,
                        CommentId = commentId,
                        ProfileId = profile.Id,
                        CreatedAt = DateTime.UtcNow,
                    });   
                }
                
                var votesCount = await _adsTopicCommentsVotesManager.CountAdsTopicCommentVotesByCommentIdAsync(commentId);

                var adsTopicComment = await _adsTopicCommentsManager.FindAdsTopicCommentById(viewModel.AdsId, commentId);

                adsTopicComment.VotesCount = votesCount;

                await _adsTopicCommentsManager.UpdateAdsTopicCommentByIdAsync(viewModel.AdsId, commentId, adsTopicComment);

                return Ok(new AdsTopicCommentVotesModel
                {
                    VoteType = viewModel.VoteType,
                    VotesCount = ((double) votesCount).ToMetric()
                });
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }
            
        [HttpGet]
        [Authorize]
        [Route("/Topics/CountAdsTopicCommentsByAdsIdAsync/{adsId}")]
        public async Task<IActionResult> CountAdsTopicCommentsByAdsIdAsync(string adsId)
        {
            try
            {
                var commentsCount = await _adsTopicCommentsManager.EstimatedAdsTopicCommentsByAdsIdAsync(adsId);
                return Ok(new AdsTopicCommentsCountModel
                {
                    AdsId = adsId,
                    CommentsCount = ((double)commentsCount).ToMetric()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }
        
        [HttpPost("AddImage")]
        [Route("/Topics/AddAdsTopicPreviewImages")]
        public async Task<IActionResult> AddAdsTopicPhotos(string returnUrl, IFormFile file)
        {
            ViewData["ReturnUrl"] = returnUrl;
                 
            if (file.Length <= 0)
            {
                //todo: return View with ModelErrors
                return BadRequest();
            }
            
            var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

            var profilePhotosDir = $"{_hostingEnvironment.WebRootPath}/images/profiles/{profile.Id.ToString()}/photos";

            if (!Directory.Exists(profilePhotosDir))
            {
                Directory.CreateDirectory(profilePhotosDir);
            }
            
            var profilePhotosDirFullPath = Path.Combine(profilePhotosDir, file.FileName);

            using (var stream = new FileStream(profilePhotosDirFullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //todo: Create profile ads photo
            
//            await _profilesImagesManager.CreateProfileImageAsync(new MongoDbProfileImage
//            {
//                ProfileId = profile.Id,
//                ImagePath = $"/images/profiles/{profile.Id}/{file.FileName}",
//                CreatedAt = DateTime.UtcNow
//            });
//
            return Ok();
        }
    }
}