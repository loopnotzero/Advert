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
                var models = new List<ArticleModel>();

                var articles = await _articlesManager.FindArticlesAsync(_configuration.GetSection("EggheadOptions").GetValue<int>("ArticlesPerPage"));

                foreach (var article in articles)
                {
                    models.Add(new ArticleModel
                    {
                        Id = article.Id.ToString(),
                        Title = article.Title,
                        Text = article.Text.Length > 1000 ? article.Text.Substring(0, 1000) + "..." : article.Text,
                        NormalizedEmail = article.NormalizedEmail,
                        LikesCount = ((double) article.LikesCount).ToMetric(),
                        DislikesCount = ((double) article.DislikesCount).ToMetric(),
                        ViewsCount = ((double) article.ViewsCount).ToMetric(),
                        CommentsCount = ((double) article.CommentsCount).ToMetric(),
                        CreatedAt = article.CreatedAt.Humanize(),
                    });
                }

                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                // ReSharper disable once InconsistentNaming
                var orderedTopicsByEngagementRate = articles.OrderByDescending(x => EngagementRate.ComputeEngagementRate(x.LikesCount, x.DislikesCount, x.SharesCount, x.CommentsCount, x.ViewsCount));

                return View(new CompositeArticleModel
                {
                    Profile = new ProfileModel
                    {
                        Name = profile.Name,
                        ArticlesCount = ((double) await _articlesManager.CountArticlesByNormalizedEmail(HttpContext.User.Identity.Name)).ToMetric(),
                        FollowingCount = ((double) 0).ToMetric()
                    },
                    Articles = articles.Select(x => new ArticleModel
                    {
                        Id = x.Id.ToString(),
                        Title = x.Title,
                        Text = x.Text.Length > 1000 ? x.Text.Substring(0, 1000) + "..." : x.Text,
                        NormalizedEmail = x.NormalizedEmail,
                        LikesCount = ((double) x.LikesCount).ToMetric(),
                        DislikesCount = ((double) x.DislikesCount).ToMetric(),
                        ViewsCount = ((double) x.ViewsCount).ToMetric(),
                        CommentsCount = ((double) x.CommentsCount).ToMetric(),
                        CreatedAt = x.CreatedAt.Humanize(),
                    }),
                    PopularTopics = orderedTopicsByEngagementRate.Select(x => new PopularTopic
                    {
                        Id = x.Id.ToString(),
                        Title = x.Title,
                        CreatedAt = x.CreatedAt.Humanize()
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
                    ArticleId = ObjectId.Parse(articleId),
                    Email = HttpContext.User.Identity.Name,
                    CreatedAt = DateTime.UtcNow
                };

                await _articlesViewsCountManager.CreateArticleViewsCountAsync(articleViewsCount);

                await _articlesManager.UpdateArticleViewsCountByArticleId(articleViewsCount.ArticleId, await _articlesViewsCountManager.CountArticleViewsCountAsync(articleViewsCount.ArticleId));

                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var article = await _articlesManager.FindArticleByIdAsync(articleViewsCount.ArticleId);

                var articles = await _articlesManager.FindArticlesAsync(_configuration.GetSection("EggheadOptions").GetValue<int>("ArticlesPerPage"));

                // ReSharper disable once InconsistentNaming
                var orderedTopicsByEngagementRate = articles.OrderByDescending(x => EngagementRate.ComputeEngagementRate(x.LikesCount, x.DislikesCount, x.SharesCount, x.CommentsCount, x.ViewsCount));

                return View(new CompositeArticleModel
                {
                    Profile = new ProfileModel
                    {
                        Name = profile.Name,
                        ArticlesCount = ((double) await _articlesManager.CountArticlesByNormalizedEmail(article.NormalizedEmail)).ToMetric(),
                        FollowingCount = ((double) 0).ToMetric()
                    },
                    Articles = new List<ArticleModel>
                    {
                        new ArticleModel
                        {
                            Id = article.Id.ToString(),
                            Title = article.Title,
                            Text = article.Text,
                            NormalizedEmail = article.NormalizedEmail,
                            LikesCount = ((double) article.LikesCount).ToMetric(),
                            DislikesCount = ((double) article.DislikesCount).ToMetric(),
                            ViewsCount = ((double) article.ViewsCount).ToMetric(),
                            CommentsCount = ((double) article.CommentsCount).ToMetric(),
                            CreatedAt = article.CreatedAt.Humanize()
                        }
                    },
                    PopularTopics = orderedTopicsByEngagementRate.Select(x => new PopularTopic
                    {
                        Id = x.Id.ToString(),
                        Title = x.Title,
                        CreatedAt = x.CreatedAt.Humanize()
                    }).ToList()
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
        public async Task<IActionResult> PublishArticleAsync([FromBody] PublishArticleModel model)
        {
            try
            {
                var article = new MongoDbArticle
                {
                    Title = model.Title,
                    Text = model.Text,
                    Email = HttpContext.User.Identity.Name,
                    ReleaseType = ReleaseType.PreModeration,
                    CreatedAt = DateTime.UtcNow,
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
        public async Task<IActionResult> CreateArticleVoteAsync([FromBody] ArticleVoteModel model)
        {
            try
            {
                var articleId = ObjectId.Parse(model.ArticleId);

                var vote = await _articlesVotesManager.FindArticleVoteByNormalizedEmailAsync(articleId, HttpContext.User.Identity.Name);

                if (vote == null)
                {
                    await _articlesVotesManager.CreateArticleVoteAsync(new MongoDbArticleVote
                    {
                        ArticleId = articleId,
                        VoteType = model.VoteType,
                        Email = HttpContext.User.Identity.Name,
                        CreatedAt = DateTime.UtcNow
                    });

                    var votesCount = await _articlesVotesManager.CountArticleVotesByVoteTypeAsync(articleId, model.VoteType);

                    switch (model.VoteType)
                    {
                        case VoteType.None:
                            var logString = $"Vote type is not valid. Article id: {model.ArticleId} Email: {HttpContext.User.Identity.Name}";
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

                    return Ok(new ArticleVotesModel
                    {
                        VoteType = model.VoteType,
                        VotesCount = ((double) votesCount).ToMetric()
                    });
                }
                else
                {
                    if (vote.VoteType == model.VoteType)
                    {
                        await _articlesVotesManager.DeleteArticleVoteByIdAsync(vote.Id);
                    }

                    var votesCount = await _articlesVotesManager.CountArticleVotesByVoteTypeAsync(articleId, model.VoteType);

                    switch (model.VoteType)
                    {
                        case VoteType.None:
                            var logString = $"Vote type is not valid. Article id: {model.ArticleId} Email: {HttpContext.User.Identity.Name}";
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

                    return Ok(new ArticleVotesModel
                    {
                        VoteType = model.VoteType,
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
        public async Task<IActionResult> CreateArticleCommentAsync([FromBody] PublishArticleCommentModel model)
        {
            try
            {
                var articleId = ObjectId.Parse(model.ArticleId);

                var collectionName = model.ArticleId;

                var articleComment = new MongoDbArticleComment
                {
                    ArticleId = articleId,
                    Text = model.Text,
                    ReplyTo = model.ReplyTo == null ? ObjectId.Empty : ObjectId.Parse(model.ReplyTo),
                    CreatedAt = DateTime.UtcNow
                };

                await _articlesCommentsManager.CreateArticleComment(collectionName, articleComment);

                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var comment = await _articlesCommentsManager.FindArticleCommentById(collectionName, articleComment.Id);

                return Ok(new ArticleCommentModel
                {
                    ArticleId = model.ArticleId,
                    CommentId = comment.Id.ToString(),
                    Name = profile.Name,
                    Text = comment.Text,
                    ReplyTo = comment.ReplyTo == ObjectId.Empty ? null : comment.ReplyTo.ToString(),
                    CreatedAt = comment.CreatedAt.Humanize()
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
        public async Task<IActionResult> CreateArticleCommentVoteAsync([FromBody] ArticleCommentVoteModel model)
        {
            try
            {
                var articleId = ObjectId.Parse(model.ArticleId);

                var commentId = ObjectId.Parse(model.CommentId);

                var vote = await _articleCommentsVotesManager.FindArticleCommentVoteAsync(commentId);

                if (vote == null)
                {
                    await _articleCommentsVotesManager.CreateArticleCommentVoteAsync(new MongoDbArticleCommentVote
                    {
                        ArticleId = articleId,
                        CommentId = commentId,
                        VoteType = model.VoteType,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    if (vote.VoteType == model.VoteType)
                    {
                        await _articleCommentsVotesManager.DeleteArticleCommentVoteAsync(vote.Id);
                    }
                }

                var votesCount = await _articleCommentsVotesManager.CountArticleCommentVotesAsync(commentId, model.VoteType);

                return Ok(new ArticleCommentVotesModel
                {
                    VoteType = model.VoteType,
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

        [HttpGet]
        [Authorize]
        [Route("/Articles/FindArticleCommentsByArticleIdAsync/{articleId}")]
        public async Task<IActionResult> FindArticleCommentsByArticleIdAsync(string articleId)
        {
            var articleComments = await _articlesCommentsManager.FindArticleCommentsByCollectionName(articleId, _configuration.GetSection("EggheadOptions").GetValue<int>("CommentsPerArticle"));

            return PartialView("ArticleCommentsPartial", articleComments.Select(x => new ArticleCommentModel
            {
                ArticleId = articleId,
                CommentId = x.Id.ToString(),
                Text = x.Text,
                ReplyTo = x.ReplyTo == ObjectId.Empty ? null : x.ReplyTo.ToString(),
                CreatedAt = x.CreatedAt.Humanize(),
            }));
        }
    }
}