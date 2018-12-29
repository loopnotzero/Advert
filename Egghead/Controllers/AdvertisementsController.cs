using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Egghead.Common.Advertisements;
using Egghead.Common.Metrics;
using Egghead.Exceptions;
using Egghead.Managers;
using Egghead.Models.Advertisements;
using Egghead.Models.Profiles;
using Egghead.MongoDbStorage.Advertisements;
using Egghead.MongoDbStorage.Profiles;
using Egghead.MongoDbStorage.Users;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Egghead.Controllers
{
    public class AdvertisementsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        private readonly AdvertisementsManager<MongoDbAdvertisement> _advertisementsManager;
        private readonly AdvertisementsLikesManager<MongoDbAdvertisementVote> _advertisementsVotesManager;
        private readonly AdvertisementsCommentsManager<MongoDbAdvertisementComment> _advertisementsCommentsManager;
        private readonly AdvertisementsViewCountManager<MongoDbAdvertisementViewsCount> _advertisementsViewsCountManager;
        private readonly AdvertisementCommentsVotesManager<MongoDbAdvertisementCommentVote> _advertisementCommentsVotesManager;
        private readonly AdvertisementCommentsVotesAggregationManager<MongoDbAdvertisementCommentVote, MongoDbAdvertisementCommentVoteAggregation> _advertisementCommentsVotesAggregationManager;
        
        private const string NoProfileImage = "/images/no_image.png";

        public AdvertisementsController(ILoggerFactory loggerFactory,
            IConfiguration configuration,
            ILookupNormalizer keyNormalizer,
            IHostingEnvironment hostingEnvironment,
            UserManager<MongoDbUser> userManager,
            ProfilesManager<MongoDbProfile> profilesManager,
            AdvertisementsManager<MongoDbAdvertisement> advertisementsManager,
            AdvertisementsLikesManager<MongoDbAdvertisementVote> advertisementsVotesManager,
            AdvertisementsCommentsManager<MongoDbAdvertisementComment> advertisementsCommentsManager,
            AdvertisementsViewCountManager<MongoDbAdvertisementViewsCount> advertisementsViewsCountManager,
            AdvertisementCommentsVotesManager<MongoDbAdvertisementCommentVote> advertisementCommentsVotesManager,
            AdvertisementCommentsVotesAggregationManager<MongoDbAdvertisementCommentVote, MongoDbAdvertisementCommentVoteAggregation> advertisementCommentsVotesAggregationManager)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;         

            _profilesManager = profilesManager;
            _advertisementsManager = advertisementsManager;
            _advertisementsVotesManager = advertisementsVotesManager;
            _advertisementsCommentsManager = advertisementsCommentsManager;
            _advertisementsViewsCountManager = advertisementsViewsCountManager;
            _advertisementCommentsVotesManager = advertisementCommentsVotesManager;
            _advertisementCommentsVotesAggregationManager = advertisementCommentsVotesAggregationManager;
        }

        [HttpGet]
        [Authorize]
        [Route("/Ads/ComposeAdvertisement")]
        public IActionResult ComposeAdvertisement()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAdvertisements([FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "keyword")] string keyword = null)
        {
            try
            {
                var howManyAdvertisementsPerPage = _configuration.GetSection("Egghead").GetValue<int>("HowManyAdvertisementsPerPage");                           
                
                var offset = (page - 1) * howManyAdvertisementsPerPage;
                                             
                List<MongoDbAdvertisement> advertisements;

                if (string.IsNullOrEmpty(keyword))
                {
                    advertisements = await _advertisementsManager.FindAdvertisementsAsync(offset, howManyAdvertisementsPerPage);
                }
                else
                {
                    advertisements = await _advertisementsManager.FindAdvertisementsWhichContainsKeywordAsync(offset, howManyAdvertisementsPerPage, keyword);
                }

                var lastPage = (long) Math.Ceiling((double) await _advertisementsManager.EstimatedAdvertisementsCountAsync() / howManyAdvertisementsPerPage);               
                
                var howManyPages = _configuration.GetSection("Egghead").GetValue<int>("HowManyPages");
                
                var middlePosition = (long) Math.Ceiling((double) howManyPages / 2);
                
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
                    if (endPage < howManyPages)
                    {
                        endPage = lastPage < howManyPages ? lastPage : howManyPages;
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
                        AdvertisementsCount = ((double) await _advertisementsManager.CountAdvertisementsByProfileIdAsync(profile.Id)).ToMetric(),
                    },

                    Advertisements = advertisements.Select(advertisement => new AdvertisementViewModel
                    {
                        AdsId = advertisement.Id.ToString(),
                        ProfileName = advertisement.ProfileName,
                        ProfileImagePath = advertisement.ProfileImagePath,
                        Text = advertisement.Text.Length > 1000 ? advertisement.Text.Substring(0, 1000) + "..." : advertisement.Text,
                        Title = advertisement.Title,
                        LikesCount = ((double) advertisement.LikesCount).ToMetric(),
                        SharesCount = ((double) 0).ToMetric(),
                        ViewsCount = ((double) advertisement.ViewsCount).ToMetric(),
                        CommentsCount = ((double) advertisement.CommentsCount).ToMetric(),
                        CreatedAt = advertisement.CreatedAt.Humanize(),
                        IsTopicOwner = advertisement.ProfileId.Equals(profile.Id)
                    }),

                    RecommendedAdvertisements = advertisements
                        .OrderByDescending(x => EngagementRate.ComputeEngagementRate(x.LikesCount, x.SharesCount, x.CommentsCount, x.ViewsCount))
                        .Select(advertisement => new RecommendedAdvertisementViewModel
                        {
                            AdsId = advertisement.Id.ToString(),
                            ProfileName = advertisement.ProfileName,
                            ProfileImagePath = advertisement.ProfileImagePath,
                            Title = advertisement.Title,
                            CreatedAt = advertisement.CreatedAt.Humanize(),
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
        [Route("/Ads/{adsId}")]
        public async Task<IActionResult> GetAdvertisementContent(string adsId, [FromQuery(Name = "page")] int page = 1)
        {
            try
            {
                var adsViewsCount = new MongoDbAdvertisementViewsCount
                {
                    Email = HttpContext.User.Identity.Name,
                    AdsId = ObjectId.Parse(adsId),
                    CreatedAt = DateTime.UtcNow
                };
                
                await _advertisementsViewsCountManager.CreateAdvertisementViewsCountAsync(adsViewsCount);
               
                var advertisement = await _advertisementsManager.FindAdvertisementByIdAsync(adsViewsCount.AdsId);
                
                advertisement.ViewsCount = await _advertisementsViewsCountManager.CountAdvertisementViewsCountAsync(adsViewsCount.AdsId);
                
                await _advertisementsManager.UpdateAdvertisementAsync(advertisement);
       
                var advertisementComments = await _advertisementsCommentsManager.FindAdvertisementCommentsAsync(adsId, 0, null, SortDefinition.Descending);
   
                var commentsReplies = new Dictionary<ObjectId, AdvertisementCommentViewModel>();

                foreach (var comments in advertisementComments.OrderBy(x => x.ReplyTo).GroupBy(comment => comment.ReplyTo))
                {
                    if (comments.Key == ObjectId.Empty)
                    {
                        foreach (var comment in comments)
                        {
                            commentsReplies.Add(comment.Id, new AdvertisementCommentViewModel
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
                        if (commentsReplies.TryGetValue(comments.Key, out var advertisementComment))
                        {
                            if (advertisementComment.Comments == null)
                            {
                                if (comments.Any())
                                {
                                    advertisementComment.Comments = comments.OrderBy(x => x.CreatedAt).Select(comment =>
                                    {
                                        var model = new AdvertisementCommentViewModel
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
                                advertisementComment.Comments.AddRange(comments.OrderBy(x => x.CreatedAt).Select(comment =>
                                {
                                    var model = new AdvertisementCommentViewModel
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

                var advertisements = await _advertisementsManager.FindAdvertisementsAsync(_configuration.GetSection("Egghead").GetValue<int>("HowManyAdvertisementsPerPage"));

                var orderedAdvertisements = advertisements.OrderByDescending(x => EngagementRate.ComputeEngagementRate(x.LikesCount, x.SharesCount, x.CommentsCount, x.ViewsCount));

                return View(new AggregatorViewModel
                {
                    Profile = new ProfileModel
                    {
                        Name = profile.Name,
                        Id = profile.Id.ToString(),
                        ImagePath = profile.ImagePath ?? NoProfileImage,
                        AdvertisementsCount = ((double) await _advertisementsManager.CountAdvertisementsByProfileIdAsync(advertisement.ProfileId)).ToMetric(),                      
                    },                   
                    Advertisements = new List<AdvertisementViewModel>
                    {
                        new AdvertisementViewModel
                        {
                            AdsId = advertisement.Id.ToString(),
                            ProfileName = advertisement.ProfileName,
                            ProfileImagePath = advertisement.ProfileImagePath,
                            Text = advertisement.Text,
                            Title = advertisement.Title,
                            LikesCount = ((double) advertisement.LikesCount).ToMetric(),
                            SharesCount = ((double)0).ToMetric(),
                            ViewsCount = ((double) advertisement.ViewsCount).ToMetric(),
                            CommentsCount = ((double) advertisement.CommentsCount).ToMetric(),
                            CreatedAt = advertisement.CreatedAt.Humanize()                           
                        }
                    },                  
                    RecommendedAdvertisements = orderedAdvertisements.Select(dbAdvertisement => new RecommendedAdvertisementViewModel
                    {
                        AdsId = dbAdvertisement.Id.ToString(),
                        ProfileName = dbAdvertisement.ProfileName,
                        ProfileImagePath = dbAdvertisement.ProfileImagePath,
                        Title = dbAdvertisement.Title,
                        CreatedAt = dbAdvertisement.CreatedAt.Humanize(),                      
                    }).ToList(),                  
                    AdvertisementsComments = commentsReplies.Values.ToList(),
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
        [Route("/Ads/PublishAdvertisementAsync")]
        public async Task<IActionResult> PublishAdvertisementAsync([FromBody] PublishAdvertisementViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var advertisement = new MongoDbAdvertisement
                {
                    Text = viewModel.Text,
                    Title = viewModel.Title,
                    ReleaseType = ReleaseType.PreModeration,
                    ProfileId = profile.Id,
                    ProfileName = profile.Name,
                    ProfileImagePath = profile.ImagePath,
                    CreatedAt = DateTime.UtcNow,
                };

                await _advertisementsManager.CreateAdvertisementAsync(advertisement);

                return Ok(new
                {
                    returnUrl = Url.Action("GetAdvertisementContent", "Advertisements", new {adsId = advertisement.Id})
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
        [Route("/Ads/GetAdvertisementByIdAsync/{adsId}")]
        public async Task<IActionResult> GetAdvertisementByIdAsync(string adsId)
        {
            try
            {
                var advertisement = await _advertisementsManager.FindAdvertisementByIdAsync(ObjectId.Parse(adsId));
                return Ok(advertisement);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("/Ads/DeleteAdvertisementByIdAsync/{adsId}")]
        public async Task<IActionResult> DeleteAdvertisementByIdAsync(string adsId)
        {
            try
            {
                await _advertisementsManager.DeleteAdvertisementByIdAsync(ObjectId.Parse(adsId));
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
        [Route("/Ads/CreateAdvertisementVoteAsync")]
        public async Task<IActionResult> CreateAdvertisementVoteAsync([FromBody] AdvertisementVoteViewModel viewModel)
        {
            try
            {
                var adsId = ObjectId.Parse(viewModel.AdsId);

                var vote = await _advertisementsVotesManager.FindAdvertisementVoteByNormalizedEmailAsync(adsId, HttpContext.User.Identity.Name);

                if (vote == null)
                {
                    await _advertisementsVotesManager.CreateAdvertisementVoteAsync(new MongoDbAdvertisementVote
                    {
                        Email = HttpContext.User.Identity.Name,
                        VoteType = viewModel.VoteType,
                        AdsId = adsId,
                        CreatedAt = DateTime.UtcNow
                    });

                    var votesCount = await _advertisementsVotesManager.CountAdvertisementVotesByVoteTypeAsync(adsId, viewModel.VoteType);

                    switch (viewModel.VoteType)
                    {
                        case VoteType.None:
                            var logString = $"Vote type is not valid. Ads id: {viewModel.AdsId} Email: {HttpContext.User.Identity.Name}";
                            throw new AdvertisementCommentVoteException(logString);
                        case VoteType.Like:
                            await _advertisementsManager.UpdateAdvertisementLikesCountByAdsId(adsId, votesCount);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return Ok(new AdvertisementVotesViewModel
                    {
                        VoteType = viewModel.VoteType,
                        VotesCount = ((double) votesCount).ToMetric()
                    });
                }
                else
                {
                    if (vote.VoteType == viewModel.VoteType)
                    {
                        await _advertisementsVotesManager.DeleteAdvertisementVoteByIdAsync(vote.Id);
                    }

                    var votesCount = await _advertisementsVotesManager.CountAdvertisementVotesByVoteTypeAsync(adsId, viewModel.VoteType);

                    switch (viewModel.VoteType)
                    {
                        case VoteType.None:
                            var logString = $"Vote type is not valid. Ads id: {viewModel.AdsId} Email: {HttpContext.User.Identity.Name}";
                            throw new AdvertisementCommentVoteException(logString);
                        case VoteType.Like:
                            await _advertisementsManager.UpdateAdvertisementLikesCountByAdsId(adsId, votesCount);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return Ok(new AdvertisementVotesViewModel
                    {
                        VoteType = viewModel.VoteType,
                        VotesCount = ((double) votesCount).ToMetric()
                    });
                }
            }
            catch (AdvertisementVoteException e)
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
        [Route("/Ads/CreateAdvertisementCommentAsync")]
        public async Task<IActionResult> CreateAdvertisementCommentAsync([FromBody] PublicCommentViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);
                
                var adsId = ObjectId.Parse(viewModel.AdsId);

                var advertisementComment = new MongoDbAdvertisementComment
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
              
                await _advertisementsCommentsManager.CreateAdvertisementComment(viewModel.AdsId, advertisementComment);
                
                var comment = await _advertisementsCommentsManager.FindAdvertisementCommentById(viewModel.AdsId, advertisementComment.Id);
                
                var advertisement = await _advertisementsManager.FindAdvertisementByIdAsync(comment.AdsId);
                
                advertisement.CommentsCount = await _advertisementsCommentsManager.EstimatedAdvertisementCommentsByAdsIdAsync(viewModel.AdsId);
                
                await _advertisementsManager.UpdateAdvertisementAsync(advertisement);

                return Ok(new AdvertisementCommentViewModel
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
        [Route("/Ads/CreateAdvertisementCommentVoteAsync")]
        public async Task<IActionResult> CreateAdvertisementCommentVoteAsync([FromBody] AdvertisementCommentVoteViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var commentId = ObjectId.Parse(viewModel.CommentId);

                var commentVote = await _advertisementCommentsVotesManager.FindAdvertisementCommentVoteOrDefaultAsync(commentId, profile.Id, null);

                if (commentVote != null)
                {
                    await _advertisementCommentsVotesManager.DeleteAdvertisementCommentVoteByIdAsync(commentVote.Id);   
                }
                else
                {
                    await _advertisementCommentsVotesManager.CreateAdvertisementCommentVoteAsync(new MongoDbAdvertisementCommentVote
                    {
                        AdsId = ObjectId.Parse(viewModel.AdsId),
                        VoteType = viewModel.VoteType,
                        CommentId = commentId,
                        ProfileId = profile.Id,
                        CreatedAt = DateTime.UtcNow,
                    });   
                }
                
                var votesCount = await _advertisementCommentsVotesManager.CountAdvertisementCommentVotesByCommentIdAsync(commentId);

                var advertisementComment = await _advertisementsCommentsManager.FindAdvertisementCommentById(viewModel.AdsId, commentId);

                advertisementComment.VotesCount = votesCount;

                await _advertisementsCommentsManager.UpdateAdvertisementCommentByIdAsync(viewModel.AdsId, commentId, advertisementComment);

                return Ok(new AdvertisementCommentVotesModel
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
        [Route("/Ads/CountAdvertisementCommentsByAdsIdAsync/{adsId}")]
        public async Task<IActionResult> CountAdvertisementCommentsByAdsIdAsync(string adsId)
        {
            try
            {
                var commentsCount = await _advertisementsCommentsManager.EstimatedAdvertisementCommentsByAdsIdAsync(adsId);
                return Ok(new AdvertisementCommentsCountModel
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
        [Route("/Ads/AddAdvertisementPreviewImages")]
        public async Task<IActionResult> AddAdvertisementPhotos(string returnUrl, IFormFile file)
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
            

            var profileAdvertisementPhotoFullPath = Path.Combine(profilePhotosDir, file.FileName);

            using (var stream = new FileStream(profileAdvertisementPhotoFullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //todo: Create profile advertisement photo
            
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