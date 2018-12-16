using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Egghead.Common.Articles;
using Egghead.Common.Metrics;
using Egghead.Exceptions;
using Egghead.Managers;
using Egghead.Models.Articles;
using Egghead.Models.Profiles;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Profiles;
using Egghead.MongoDbStorage.Users;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Egghead.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        private readonly ArticlesManager<MongoDbArticle> _articlesManager;
        private readonly ArticlesLikesManager<MongoDbArticleVote> _articlesVotesManager;
        private readonly ArticlesCommentsManager<MongoDbArticleComment> _articlesCommentsManager;
        private readonly ArticlesViewCountManager<MongoDbArticleViewsCount> _articlesViewsCountManager;
        private readonly ArticleCommentsVotesManager<MongoDbArticleCommentVote> _articleCommentsVotesManager;
        private readonly ArticleCommentsVotesAggregationManager<MongoDbArticleCommentVote, MongoDbArticleCommentVoteAggregation> _articleCommentsVotesAggregationManager;
        
        private const string NoProfileImage = "/images/no_image.png";

        public ArticlesController(ILoggerFactory loggerFactory,
            IConfiguration configuration,
            ILookupNormalizer keyNormalizer,
            UserManager<MongoDbUser> userManager,
            ProfilesManager<MongoDbProfile> profilesManager,
            ArticlesManager<MongoDbArticle> articlesManager,
            ArticlesLikesManager<MongoDbArticleVote> articlesVotesManager,
            ArticlesCommentsManager<MongoDbArticleComment> articlesCommentsManager,
            ArticlesViewCountManager<MongoDbArticleViewsCount> articlesViewsCountManager,
            ArticleCommentsVotesManager<MongoDbArticleCommentVote> articleCommentsVotesManager,
            ArticleCommentsVotesAggregationManager<MongoDbArticleCommentVote, MongoDbArticleCommentVoteAggregation> articleCommentsVotesAggregationManager)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _configuration = configuration;
            _profilesManager = profilesManager;
            _articlesManager = articlesManager;
            _articlesVotesManager = articlesVotesManager;
            _articlesCommentsManager = articlesCommentsManager;
            _articlesViewsCountManager = articlesViewsCountManager;
            _articleCommentsVotesManager = articleCommentsVotesManager;
            _articleCommentsVotesAggregationManager = articleCommentsVotesAggregationManager;
        }

        [HttpGet]
        [Authorize]
        [Route("/Articles/ComposeArticle")]
        public IActionResult ComposeArticle()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        [Route("/{currentPage?}")]
        public async Task<IActionResult> GetArticles(int currentPage = 1)
        {
            try
            {
                var articlesPerPage = _configuration.GetSection("Egghead").GetValue<int>("MaxArticlesPerPage");
                                      
                var offset = (currentPage - 1) * articlesPerPage;

                var articles = await _articlesManager.FindArticlesAsync(offset, articlesPerPage);

                var shift = (long) Math.Ceiling((double) articlesPerPage / 2);
                
                var beginPage = currentPage - articlesPerPage + shift;                    
                var endPage = currentPage + shift;
                var lastPage = (long) Math.Ceiling((double) await _articlesManager.EstimatedArticlesCountAsync() / articlesPerPage);

                if (beginPage < 0)
                {
                    beginPage = 1;                    
                    endPage = articlesPerPage + 1;
                }
                else
                {
                    if (endPage > lastPage)
                    {
                        beginPage = lastPage - articlesPerPage + 1;                        
                        endPage = lastPage + 1;
                    } 
                }
                
                
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);
             
                return View(new AggregatorViewModel
                {
                    Profile = new ProfileModel
                    {
                        Id = profile.Id.ToString(),
                        Name = profile.Name,
                        ImagePath = profile.ImagePath ?? NoProfileImage,
                        ArticlesCount = ((double) await _articlesManager.CountArticlesByProfileIdAsync(profile.Id)).ToMetric(),
                        FollowingCount = ((double) 0).ToMetric()
                    },

                    Articles = articles.Select(article => new ArticleViewModel
                    {
                        ArticleId = article.Id.ToString(),
                        ProfileName = article.ProfileName,
                        ProfileImagePath = article.ProfileImagePath,
                        Text = article.Text.Length > 1000 ? article.Text.Substring(0, 1000) + "..." : article.Text,
                        Title = article.Title,
                        LikesCount = ((double) article.LikesCount).ToMetric(),
                        SharesCount = ((double) 0).ToMetric(),
                        ViewsCount = ((double) article.ViewsCount).ToMetric(),
                        CommentsCount = ((double) article.CommentsCount).ToMetric(),
                        CreatedAt = article.CreatedAt.Humanize(),
                    }),

                    PopularArticles = articles
                        .OrderByDescending(x => EngagementRate.ComputeEngagementRate(x.LikesCount, x.SharesCount, x.CommentsCount, x.ViewsCount))
                        .Select(popularArticle => new PopularArticleViewModel
                        {
                            ArticleId = popularArticle.Id.ToString(),
                            ProfileName = popularArticle.ProfileName,
                            ProfileImagePath = popularArticle.ProfileImagePath,
                            Title = popularArticle.Title,
                            CreatedAt = popularArticle.CreatedAt.Humanize(),
                        }).ToList(),

                    BeginPage = beginPage,
                    EndPage = endPage,
                    CurrentPage = currentPage,
                    LastPage = (long) totalPages
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
        [Route("/Articles/{articleId}/{currentPage?}")]
        public async Task<IActionResult> GetArticleContent(string articleId, int pageIndex = 1)
        {
            try
            {
                var articleViewsCount = new MongoDbArticleViewsCount
                {
                    Email = HttpContext.User.Identity.Name,
                    ArticleId = ObjectId.Parse(articleId),
                    CreatedAt = DateTime.UtcNow
                };
                
                await _articlesViewsCountManager.CreateArticleViewsCountAsync(articleViewsCount);
               
                var article = await _articlesManager.FindArticleByIdAsync(articleViewsCount.ArticleId);
                
                article.ViewsCount = await _articlesViewsCountManager.CountArticleViewsCountAsync(articleViewsCount.ArticleId);
                
                await _articlesManager.UpdateArticleAsync(article);
       
                var articleComments = await _articlesCommentsManager.FindArticleCommentsAsync(articleId, 0, null, SortDefinition.Descending);
   
                var commentsReplies = new Dictionary<ObjectId, ArticleCommentViewModel>();

                foreach (var comments in articleComments.OrderBy(x => x.ReplyTo).GroupBy(comment => comment.ReplyTo))
                {
                    if (comments.Key == ObjectId.Empty)
                    {
                        foreach (var comment in comments)
                        {
                            commentsReplies.Add(comment.Id, new ArticleCommentViewModel
                            {
                                Text = comment.Text,
                                ReplyTo = comment.ReplyTo.ToString(),
                                CommentId = comment.Id.ToString(),
                                ArticleId = comment.ArticleId.ToString(),
                                CreatedAt = comment.CreatedAt.Humanize(),
                                ProfileName = comment.ProfileName,
                                ProfileImagePath = comment.ProfileImagePath ?? NoProfileImage,
                                VotesCount = ((double) comment.VotesCount).ToMetric()
                            });
                        }
                    }
                    else
                    {
                        if (commentsReplies.TryGetValue(comments.Key, out var articleComment))
                        {
                            if (articleComment.Comments == null)
                            {
                                if (comments.Any())
                                {
                                    articleComment.Comments = comments.OrderBy(x => x.CreatedAt).Select(comment =>
                                    {
                                        var model = new ArticleCommentViewModel
                                        {
                                            Text = comment.Text,
                                            ReplyTo = comment.ReplyTo.ToString(),
                                            CommentId = comment.Id.ToString(),
                                            ArticleId = comment.ArticleId.ToString(),
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
                                articleComment.Comments.AddRange(comments.OrderBy(x => x.CreatedAt).Select(comment =>
                                {
                                    var model = new ArticleCommentViewModel
                                    {
                                        Text = comment.Text,
                                        ReplyTo = comment.ReplyTo.ToString(),
                                        CommentId = comment.Id.ToString(),
                                        ArticleId = comment.ArticleId.ToString(),
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

                //todo: Find articles by engagement rate with category list param

                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var articles = await _articlesManager.FindArticlesAsync(_configuration.GetSection("Egghead").GetValue<int>("MaxArticles"));

                var popularArticles = articles.OrderByDescending(x => EngagementRate.ComputeEngagementRate(x.LikesCount, x.SharesCount, x.CommentsCount, x.ViewsCount));

                return View(new AggregatorViewModel
                {
                    Profile = new ProfileModel
                    {
                        Name = profile.Name,
                        Id = profile.Id.ToString(),
                        ImagePath = profile.ImagePath ?? NoProfileImage,
                        ArticlesCount = ((double) await _articlesManager.CountArticlesByProfileIdAsync(article.ProfileId)).ToMetric(),
                        FollowingCount = ((double) 0).ToMetric()
                    },                   
                    Articles = new List<ArticleViewModel>
                    {
                        new ArticleViewModel
                        {
                            ArticleId = article.Id.ToString(),
                            ProfileName = article.ProfileName,
                            ProfileImagePath = article.ProfileImagePath,
                            Text = article.Text,
                            Title = article.Title,
                            LikesCount = ((double) article.LikesCount).ToMetric(),
                            SharesCount = ((double)0).ToMetric(),
                            ViewsCount = ((double) article.ViewsCount).ToMetric(),
                            CommentsCount = ((double) article.CommentsCount).ToMetric(),
                            CreatedAt = article.CreatedAt.Humanize()                           
                        }
                    },                  
                    PopularArticles = popularArticles.Select(popularArticle => new PopularArticleViewModel
                    {
                        ArticleId = popularArticle.Id.ToString(),
                        ProfileName = popularArticle.ProfileName,
                        ProfileImagePath = popularArticle.ProfileImagePath,
                        Title = popularArticle.Title,
                        CreatedAt = popularArticle.CreatedAt.Humanize(),                      
                    }).ToList(),                  
                    ArticleComments = commentsReplies.Values.ToList(),
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
        [Route("/Articles/PublishArticleAsync")]
        public async Task<IActionResult> PublishArticleAsync([FromBody] PublishArticleViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var article = new MongoDbArticle
                {
                    Text = viewModel.Text,
                    Title = viewModel.Title,
                    ReleaseType = ReleaseType.PreModeration,
                    ProfileId = profile.Id,
                    ProfileName = profile.Name,
                    ProfileImagePath = profile.ImagePath,
                    CreatedAt = DateTime.UtcNow,
                };

                await _articlesManager.CreateArticleAsync(article);

                var url = Url.Action("GetArticleContent", "Articles", new {articleId = article.Id});

                return Ok(new
                {
                    returnUrl = url
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
        [Route("/Articles/GetArticleByIdAsync/{articleId}")]
        public async Task<IActionResult> GetArticleByIdAsync(string articleId)
        {
            try
            {
                var article = await _articlesManager.FindArticleByIdAsync(ObjectId.Parse(articleId));
                return Ok(article);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("/Articles/DeleteArticleByIdAsync/{articleId}")]
        public async Task<IActionResult> DeleteArticleByIdAsync(string articleId)
        {
            try
            {
                await _articlesManager.DeleteArticleByIdAsync(ObjectId.Parse(articleId));
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
        [Route("/Articles/CreateArticleVoteAsync")]
        public async Task<IActionResult> CreateArticleVoteAsync([FromBody] ArticleVoteViewModel viewModel)
        {
            try
            {
                var articleId = ObjectId.Parse(viewModel.ArticleId);

                var vote = await _articlesVotesManager.FindArticleVoteByNormalizedEmailAsync(articleId, HttpContext.User.Identity.Name);

                if (vote == null)
                {
                    await _articlesVotesManager.CreateArticleVoteAsync(new MongoDbArticleVote
                    {
                        Email = HttpContext.User.Identity.Name,
                        VoteType = viewModel.VoteType,
                        ArticleId = articleId,
                        CreatedAt = DateTime.UtcNow
                    });

                    var votesCount = await _articlesVotesManager.CountArticleVotesByVoteTypeAsync(articleId, viewModel.VoteType);

                    switch (viewModel.VoteType)
                    {
                        case VoteType.None:
                            var logString = $"Vote type is not valid. Article id: {viewModel.ArticleId} Email: {HttpContext.User.Identity.Name}";
                            throw new ArticleCommentVoteException(logString);
                        case VoteType.Like:
                            await _articlesManager.UpdateArticleLikesCountByArticleId(articleId, votesCount);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return Ok(new ArticleVotesViewModel
                    {
                        VoteType = viewModel.VoteType,
                        VotesCount = ((double) votesCount).ToMetric()
                    });
                }
                else
                {
                    if (vote.VoteType == viewModel.VoteType)
                    {
                        await _articlesVotesManager.DeleteArticleVoteByIdAsync(vote.Id);
                    }

                    var votesCount = await _articlesVotesManager.CountArticleVotesByVoteTypeAsync(articleId, viewModel.VoteType);

                    switch (viewModel.VoteType)
                    {
                        case VoteType.None:
                            var logString = $"Vote type is not valid. Article id: {viewModel.ArticleId} Email: {HttpContext.User.Identity.Name}";
                            throw new ArticleCommentVoteException(logString);
                        case VoteType.Like:
                            await _articlesManager.UpdateArticleLikesCountByArticleId(articleId, votesCount);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return Ok(new ArticleVotesViewModel
                    {
                        VoteType = viewModel.VoteType,
                        VotesCount = ((double) votesCount).ToMetric()
                    });
                }
            }
            catch (ArticleVoteException e)
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
        [Route("/Articles/CreateArticleCommentAsync")]
        public async Task<IActionResult> CreateArticleCommentAsync([FromBody] PublicCommentViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);
                
                var articleId = ObjectId.Parse(viewModel.ArticleId);

                var articleComment = new MongoDbArticleComment
                {
                    Text = viewModel.Text,
                    ReplyTo = viewModel.ReplyTo == null ? ObjectId.Empty : ObjectId.Parse(viewModel.ReplyTo),
                    CreatedAt = DateTime.UtcNow,
                    ArticleId = articleId,
                    ProfileId = profile.Id,
                    ProfileName = profile.Name,
                    ProfileImagePath = profile.ImagePath ?? NoProfileImage,
                    VotesCount = 0,
                };
              
                await _articlesCommentsManager.CreateArticleComment(viewModel.ArticleId, articleComment);
                
                var comment = await _articlesCommentsManager.FindArticleCommentById(viewModel.ArticleId, articleComment.Id);
                
                var article = await _articlesManager.FindArticleByIdAsync(comment.ArticleId);
                
                article.CommentsCount = await _articlesCommentsManager.EstimatedArticleCommentsByArticleIdAsync(viewModel.ArticleId);
                
                await _articlesManager.UpdateArticleAsync(article);

                return Ok(new ArticleCommentViewModel
                {
                    Text = comment.Text,
                    ReplyTo = comment.ReplyTo == ObjectId.Empty ? null : comment.ReplyTo.ToString(),
                    CreatedAt = comment.CreatedAt.Humanize(),
                    CommentId = comment.Id.ToString(),
                    ArticleId = viewModel.ArticleId,
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
        [Route("/Articles/CreateArticleCommentVoteAsync")]
        public async Task<IActionResult> CreateArticleCommentVoteAsync([FromBody] ArticleCommentVoteViewModel viewModel)
        {
            try
            {
                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var commentId = ObjectId.Parse(viewModel.CommentId);

                var commentVote = await _articleCommentsVotesManager.FindArticleCommentVoteOrDefaultAsync(commentId, profile.Id, null);

                if (commentVote != null)
                {
                    await _articleCommentsVotesManager.DeleteArticleCommentVoteByIdAsync(commentVote.Id);   
                }
                else
                {
                    await _articleCommentsVotesManager.CreateArticleCommentVoteAsync(new MongoDbArticleCommentVote
                    {
                        VoteType = viewModel.VoteType,
                        ArticleId = ObjectId.Parse(viewModel.ArticleId),
                        CommentId = commentId,
                        ProfileId = profile.Id,
                        CreatedAt = DateTime.UtcNow,
                    });   
                }
                
                var votesCount = await _articleCommentsVotesManager.CountArticleCommentVotesByCommentIdAsync(commentId);

                var articleComment = await _articlesCommentsManager.FindArticleCommentById(viewModel.ArticleId, commentId);

                articleComment.VotesCount = votesCount;

                await _articlesCommentsManager.UpdateArticleCommentByIdAsync(viewModel.ArticleId, commentId, articleComment);

                return Ok(new ArticleCommentVotesModel
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
        [Route("/Articles/CountArticleCommentsByArticleIdAsync/{articleId}")]
        public async Task<IActionResult> CountArticleCommentsByArticleIdAsync(string articleId)
        {
            try
            {
                var commentsCount = await _articlesCommentsManager.EstimatedArticleCommentsByArticleIdAsync(articleId);
                return Ok(new ArticleCommentsCountModel
                {
                    ArticleId = articleId,
                    CommentsCount = ((double)commentsCount).ToMetric()
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