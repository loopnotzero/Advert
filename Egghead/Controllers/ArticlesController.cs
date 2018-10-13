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

        public ArticlesController(ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer, IConfiguration configuration, UserManager<MongoDbUser> userManager, ProfilesManager<MongoDbProfile> profilesManager, ArticlesManager<MongoDbArticle> articlesManager, ArticlesLikesManager<MongoDbArticleVote> articlesVotesManager, ArticlesCommentsManager<MongoDbArticleComment> articlesCommentsManager, ArticlesViewCountManager<MongoDbArticleViewsCount> articlesViewsCountManager, ArticleCommentsVotesManager<MongoDbArticleCommentVote> articleCommentsVotesManager)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _configuration = configuration;
            _profilesManager = profilesManager;
            _articlesManager = articlesManager;
            _articlesVotesManager = articlesVotesManager;
            _articlesCommentsManager = articlesCommentsManager;
            _articlesViewsCountManager = articlesViewsCountManager;
            _articleCommentsVotesManager = articleCommentsVotesManager;
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
        public async Task<IActionResult> Articles()
        {
            try
            {
                var articles = await _articlesManager.FindArticlesAsync(_configuration.GetSection("EggheadOptions").GetValue<int>("ArticlesPerPage"));
                
                var viewModels = new List<ArticleViewModel>();

                foreach (var article in articles)
                {
                    viewModels.Add(new ArticleViewModel
                    {
                        Text = article.Text.Length > 1000 ? article.Text.Substring(0, 1000) + "..." : article.Text,
                        Title = article.Title,
                        ArticleId = article.Id.ToString(),
                        CreatedAt = article.CreatedAt.Humanize(),
                        LikesCount = ((double) article.LikesCount).ToMetric(),
                        ViewsCount = ((double) article.ViewsCount).ToMetric(),
                        ProfileName = article.ProfileName,
                        DislikesCount = ((double) article.DislikesCount).ToMetric(),
                        CommentsCount = ((double) article.CommentsCount).ToMetric(),
                    });
                }

                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                // ReSharper disable once InconsistentNaming
                var orderedTopicsByEngagementRate = articles.OrderByDescending(x => EngagementRate.ComputeEngagementRate(x.LikesCount, x.DislikesCount, x.SharesCount, x.CommentsCount, x.ViewsCount));

                return View(new AggregatorViewModel
                {
                    Profile = new ProfileModel
                    {
                        ProfileName = profile.Name,
                        ProfileId = profile.Id.ToString(),
                        ProfilePhoto = profile.PhotoPath,
                        ArticlesCount = ((double) await _articlesManager.CountArticlesByProfileId(profile.Id)).ToMetric(),
                        FollowingCount = ((double) 0).ToMetric()
                    },
                    Articles = articles.Select(article => new ArticleViewModel
                    {
                        Text = article.Text.Length > 1000 ? article.Text.Substring(0, 1000) + "..." : article.Text,
                        Title = article.Title,
                        ArticleId = article.Id.ToString(),
                        CreatedAt = article.CreatedAt.Humanize(),
                        LikesCount = ((double) article.LikesCount).ToMetric(),
                        ViewsCount = ((double) article.ViewsCount).ToMetric(),
                        ProfileName = article.ProfileName,
                        DislikesCount = ((double) article.DislikesCount).ToMetric(),
                        CommentsCount = ((double) article.CommentsCount).ToMetric(),
                    }),
                    PopularArticles = orderedTopicsByEngagementRate.Select(article => new PopularArticleViewModel
                    {
                        Title = article.Title,
                        ArticleId = article.Id.ToString(),
                        CreatedAt = article.CreatedAt.Humanize(),
                        ProfileName = article.ProfileName
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
        [Route("/Articles/{articleId}")]
        public async Task<IActionResult> ArticleContent(string articleId)
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

                await _articlesManager.UpdateArticleViewsCountByArticleId(articleViewsCount.ArticleId, await _articlesViewsCountManager.CountArticleViewsCountAsync(articleViewsCount.ArticleId));

                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var article = await _articlesManager.FindArticleByIdAsync(articleViewsCount.ArticleId);

                var articles = await _articlesManager.FindArticlesAsync(_configuration.GetSection("EggheadOptions").GetValue<int>("ArticlesPerPage"));

                var articleComments = await _articlesCommentsManager.FindArticleCommentsByCollectionName(articleId, _configuration.GetSection("EggheadOptions").GetValue<int>("CommentsPerArticle"));
   
                var commentsReplies = new Dictionary<ObjectId, ArticleCommentViewModel>();

                foreach (var comments in articleComments.GroupBy(comment => comment.ReplyTo))
                {
                    if (comments.Key == ObjectId.Empty)
                    {
                        foreach (var comment in comments.OrderByDescending(x => x.CreatedAt))
                        {
                            commentsReplies.Add(comment.Id, new ArticleCommentViewModel
                            {
                                Text = comment.Text,
                                ReplyTo = comment.ReplyTo.ToString(),
                                CommentId = comment.Id.ToString(),
                                ArticleId = comment.ArticleId.ToString(),
                                CreatedAt = comment.CreatedAt.Humanize(),
                                ProfileName = comment.ProfileName,
                                ProfilePhoto = comment.ProfilePhoto,
                                VotingPoints = ((double) comment.VotingPoints).ToMetric()
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
                                    articleComment.Comments = comments.OrderByDescending(x => x.CreatedAt).Select(comment =>
                                    {
                                        var model = new ArticleCommentViewModel
                                        {
                                            Text = comment.Text,
                                            ReplyTo = comment.ReplyTo.ToString(),
                                            CommentId = comment.Id.ToString(),
                                            ArticleId = comment.ArticleId.ToString(),
                                            CreatedAt = comment.CreatedAt.Humanize(),
                                            ProfileName = comment.ProfileName,
                                            ProfilePhoto = comment.ProfilePhoto,
                                            VotingPoints = ((double) comment.VotingPoints).ToMetric()
                                        };
                                        return model;
                                    }).ToList();
                                }
                            }
                            else
                            {
                                articleComment.Comments.AddRange(comments.OrderByDescending(x => x.CreatedAt).Select(comment =>
                                {
                                    var model = new ArticleCommentViewModel
                                    {
                                        Text = comment.Text,
                                        ReplyTo = comment.ReplyTo.ToString(),
                                        CommentId = comment.Id.ToString(),
                                        ArticleId = comment.ArticleId.ToString(),
                                        CreatedAt = comment.CreatedAt.Humanize(),
                                        ProfileName = comment.ProfileName,
                                        ProfilePhoto = comment.ProfilePhoto,
                                        VotingPoints = ((double) comment.VotingPoints).ToMetric()
                                    };
                                    return model;
                                }));
                            }
                        }
                    }
                }

                var popularArticles = articles.OrderByDescending(x => EngagementRate.ComputeEngagementRate(x.LikesCount, x.DislikesCount, x.SharesCount, x.CommentsCount, x.ViewsCount));

                return View(new AggregatorViewModel
                {
                    Profile = new ProfileModel
                    {
                        ProfileName = profile.Name,
                        ProfileId = profile.Id.ToString(),
                        ProfilePhoto = profile.PhotoPath,
                        ArticlesCount = ((double) await _articlesManager.CountArticlesByProfileId(article.ProfileId)).ToMetric(),
                        FollowingCount = ((double) 0).ToMetric()
                    },
                    
                    Articles = new List<ArticleViewModel>
                    {
                        new ArticleViewModel
                        {
                            Text = article.Text,
                            Title = article.Title,
                            ArticleId = article.Id.ToString(),
                            CreatedAt = article.CreatedAt.Humanize(),
                            LikesCount = ((double) article.LikesCount).ToMetric(),
                            ViewsCount = ((double) article.ViewsCount).ToMetric(),
                            ProfileName = article.ProfileName,
                            DislikesCount = ((double) article.DislikesCount).ToMetric(),
                            CommentsCount = ((double) article.CommentsCount).ToMetric(),
                        }
                    },
                    
                    PopularArticles = popularArticles.Select(popularArticle => new PopularArticleViewModel
                    {
                        Title = popularArticle.Title,
                        ArticleId = popularArticle.Id.ToString(),
                        CreatedAt = popularArticle.CreatedAt.Humanize(),
                        ProfileName = popularArticle.ProfileName
                    }).ToList(),
                    
                    ArticleComments = commentsReplies.Values.ToList()
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
                    CreatedAt = DateTime.UtcNow,
                    ProfileId = profile.Id,
                    ProfileName = profile.Name,
                    ReleaseType = ReleaseType.PreModeration,
                };

                await _articlesManager.CreateArticleAsync(article);

                var url = Url.Action("ArticleContent", "Articles", new {articleId = article.Id});

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
                        case VoteType.Dislike:
                            await _articlesManager.UpdateArticleDislikesCountByArticleId(articleId, votesCount);
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
                        case VoteType.Dislike:
                            await _articlesManager.UpdateArticleDislikesCountByArticleId(articleId, votesCount);
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
                    ProfileName = profile.Name,
                    ProfilePhoto = profile.PhotoPath,
                    VotingPoints = 0,
                };
              
                var collectionName = viewModel.ArticleId;

                await _articlesCommentsManager.CreateArticleComment(collectionName, articleComment);

                var comment = await _articlesCommentsManager.FindArticleCommentById(collectionName, articleComment.Id);

                return Ok(new ArticleCommentViewModel
                {
                    Text = comment.Text,
                    ReplyTo = comment.ReplyTo == ObjectId.Empty ? null : comment.ReplyTo.ToString(),
                    CreatedAt = comment.CreatedAt.Humanize(),
                    CommentId = comment.Id.ToString(),
                    ArticleId = viewModel.ArticleId,
                    ProfileName = comment.ProfileName,
                    ProfilePhoto = comment.ProfilePhoto,     
                    VotingPoints = ((double)comment.VotingPoints).ToMetric()
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
                var articleId = ObjectId.Parse(viewModel.ArticleId);

                var commentId = ObjectId.Parse(viewModel.CommentId);

                var vote = await _articleCommentsVotesManager.FindArticleCommentVoteAsync(commentId);

                if (vote == null)
                {
                    await _articleCommentsVotesManager.CreateArticleCommentVoteAsync(new MongoDbArticleCommentVote
                    {
                        VoteType = viewModel.VoteType,
                        ArticleId = articleId,
                        CommentId = commentId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    if (vote.VoteType == viewModel.VoteType)
                    {
                        await _articleCommentsVotesManager.DeleteArticleCommentVoteAsync(vote.Id);
                    }
                }

                var votesCount = await _articleCommentsVotesManager.CountArticleCommentVotesAsync(commentId, viewModel.VoteType);

                return Ok(new ArticleCommentVotesModel
                {
                    VoteType = viewModel.VoteType,
                    VotesCount = ((double) votesCount).ToMetric()
                });
            }
            catch (ArticleCommentVoteException e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }
    }
}