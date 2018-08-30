using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
using Egghead.Common.Profile;
using Egghead.Exceptions;
using Egghead.Managers;
using Egghead.Models.Articles;
using Egghead.Models.Errors;
using Egghead.Models.Profile;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Users;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Egghead.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ILogger _logger;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly UserManager<MongoDbUser> _userManager;
        private readonly ArticlesManager<MongoDbArticle> _articlesManager;
        private readonly ArticlesLikesManager<MongoDbArticleVote> _articlesVotesManager;
        private readonly ArticlesCommentsManager<MongoDbArticleComment> _articlesCommentsManager;
        private readonly ArticlesViewCountManager<MongoDbArticleViewCount> _articlesViewCountManager;
        private readonly ArticleCommentsVotesManager<MongoDbArticleCommentVote> _articleCommentsVotesManager;
           
        public ArticlesController(UserManager<MongoDbUser> userManager,
            ArticlesManager<MongoDbArticle> articlesManager,
            ArticlesLikesManager<MongoDbArticleVote> articlesVotesManager,
            ArticlesCommentsManager<MongoDbArticleComment> articlesCommentsManager,
            ArticlesViewCountManager<MongoDbArticleViewCount> articlesViewCountManager,
            ArticleCommentsVotesManager<MongoDbArticleCommentVote> articleCommentsVotesManager, 
            ILoggerFactory loggerFactory,
            ILookupNormalizer keyNormalizer
        )
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _keyNormalizer = keyNormalizer;
            _userManager = userManager;
            _articlesManager = articlesManager;
            _articlesVotesManager = articlesVotesManager;
            _articlesCommentsManager = articlesCommentsManager;
            _articlesViewCountManager = articlesViewCountManager;
            _articleCommentsVotesManager = articleCommentsVotesManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult WriteArticle()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetArticleContent(string articleId)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                var objectId = ObjectId.Parse(articleId);

                await _articlesViewCountManager.CreateArticleViewCountAsync(new MongoDbArticleViewCount
                {
                    ByWho = HttpContext.User.Identity.Name,
                    ByWhoNormalized = _keyNormalizer.Normalize(HttpContext.User.Identity.Name),
                    ArticleId = objectId,
                    CreatedAt = DateTime.UtcNow
                });

                var article = await _articlesManager.FindArticleByIdAsync(objectId);

                var model = new ArticlePreviewModel
                {
                    Id = article.Id.ToString(),
                    Title = article.Title,
                    Text = article.Text,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = article.CreatedAt
                };

                return View(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Ok(new ErrorModel
                {
                    ErrorStatusCode = ErrorStatusCode.InternalServerError
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetArticlesPreview()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                var articlesPreview = new List<ArticlePreviewModel>();
                
                foreach (var article in await _articlesManager.FindArticlesAsync(50))
                {
                    articlesPreview.Add(new ArticlePreviewModel
                    {
                        Id = article.Id.ToString(),
                        Title = article.Title,
                        Text = article.Text,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CreatedAt = article.CreatedAt,
                        Likes = await _articlesVotesManager.CountArticleVotesAsync(article.Id, VoteType.Like),
                        Dislikes = await _articlesVotesManager.CountArticleVotesAsync(article.Id, VoteType.Dislike),
                        ViewCount = await _articlesViewCountManager.CountArticleViewCountAsync(article.Id),
                        CommentsCount = await _articlesCommentsManager.CountArticleCommentsByArticleIdAsync(article.Id.ToString()),
                    });
                }

                var articlesId = await _articlesViewCountManager.GetPopularArticlesByViewsCount(5);
                     
                var popularArticles = new List<PopularArticleModel>();
                
                foreach (var articleId in articlesId)
                {
                    var article = await _articlesManager.FindArticleByIdAsync(articleId);
                    popularArticles.Add(new PopularArticleModel
                    {
                        Id = article.Id.ToString(),
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Title = article.Title,
                        CreatedAt = article.CreatedAt
                    });
                }
                
                return View(new AggregationModel
                {
                    ArticlesPreview = articlesPreview,
                    PopularArticles = popularArticles,
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Ok(new ErrorModel
                {
                    ErrorStatusCode = ErrorStatusCode.InternalServerError
                });
            }
        }

        
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticleAsync([FromBody] ArticlePreviewModel previewModel)
        {
            try
            {
                await _articlesManager.CreateArticleAsync(new MongoDbArticle
                {
                    Title = previewModel.Title,
                    NormalizedTitle = _keyNormalizer.Normalize(previewModel.Title),
                    Text = previewModel.Text,
                    ByWho = HttpContext.User.Identity.Name,
                    ByWhoNormalized = _keyNormalizer.Normalize(HttpContext.User.Identity.Name),
                    CreatedAt = DateTime.UtcNow,
                    ReleaseType = ReleaseType.PreModeration,
                });

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);

                return Ok(new ErrorModel
                {
                    TagName = "",
                    RedirectUrl = "/Redirect_To_Error_Page",
                    ErrorMessage = e.Message,
                    ErrorStatusCode = ErrorStatusCode.InternalServerError,
                });
            }
        }

        [HttpGet]
        [Authorize]
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
                return Ok(new ErrorModel
                {
                    ErrorStatusCode = ErrorStatusCode.InternalServerError
                });
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UdpateArticleByIdAsync(string articleId, [FromBody] ArticlePreviewModel previewModel)
        {
            try
            {
                await _articlesManager.UpdateArticleByIdAsync(ObjectId.Parse(articleId), new MongoDbArticle
                {
                    Title = previewModel.Title,
                    NormalizedTitle = _keyNormalizer.Normalize(previewModel.Title),
                    Text = previewModel.Text,
                    ChangedAt = DateTime.UtcNow
                });

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Ok(new ErrorModel
                {
                    ErrorStatusCode = ErrorStatusCode.InternalServerError
                });
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UdpateArticleByTitleAsync(string title, [FromBody] ArticlePreviewModel previewModel)
        {
            try
            {
                await _articlesManager.UpdateArticleByTitleAsync(title, new MongoDbArticle
                {
                    Title = previewModel.Title,
                    NormalizedTitle = _keyNormalizer.Normalize(previewModel.Title),
                    Text = previewModel.Text,
                    ChangedAt = DateTime.UtcNow,
                });

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Ok(new ErrorModel
                {
                    ErrorStatusCode = ErrorStatusCode.InternalServerError
                });
            }
        }

        [HttpDelete]
        [Authorize]
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
                return Ok(new ErrorModel
                {
                    ErrorStatusCode = ErrorStatusCode.InternalServerError
                });
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteArticleByTitleAsync(string title)
        {
            await _articlesManager.DeleteArticleByTitleAsync(title);
            return Ok();
        }

        
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticleVoteAsync(string articleId, [FromBody] ArticleVoteModel model)
        {
            try
            {
                var objectId = ObjectId.Parse(articleId);

                var articleVote = await _articlesVotesManager.FindArticleVoteAsync(objectId, model.VoteType, HttpContext.User.Identity.Name);

                if (articleVote == null)
                {
                    await _articlesVotesManager.CreateArticleVoteAsync(new MongoDbArticleVote
                    {
                        ByWho = HttpContext.User.Identity.Name,
                        ByWhoNormalized = _keyNormalizer.Normalize(HttpContext.User.Identity.Name),
                        ArticleId = objectId,
                        VoteType = model.VoteType,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                var articleVotes = await _articlesVotesManager.CountArticleVotesAsync(objectId, model.VoteType);

                return Ok(articleVotes);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Ok(new ErrorModel
                {
                    ErrorStatusCode = ErrorStatusCode.InternalServerError
                });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpsertArticleCommentVoteAsync(string articleId, [FromBody] ArticleCommentVoteModel model)
        {
            try
            {
                var articleIdObj = ObjectId.Parse(articleId);
                var commentIdObj = ObjectId.Parse(model.CommentId);

                if (model.VoteType == VoteType.None)
                {
                    var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                    var logString = $"Upsert vote type is not valid. Article id: {articleId} By Who: {user.Email}";
                    throw new ArticleCommentVoteException(logString);
                }

                var articleCommentVote = await _articleCommentsVotesManager.FindArticleCommentVoteAsync(articleIdObj, commentIdObj);

                if (articleCommentVote == null)
                {
                    await _articleCommentsVotesManager.CreateArticleCommentVoteAsync(new MongoDbArticleCommentVote
                    {
                        ByWho = HttpContext.User.Identity.Name,
                        ByWhoNormalized = _keyNormalizer.Normalize(HttpContext.User.Identity.Name),
                        ArticleId = articleIdObj,
                        CommentId = commentIdObj,
                        VoteType = model.VoteType,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    if (model.VoteType == articleCommentVote.VoteType)
                    {
                        await _articleCommentsVotesManager.DeleteArticleCommentVoteAsync(articleCommentVote.Id);
                    }
                    else
                    {
                        switch (articleCommentVote.VoteType)
                        {
                            case VoteType.None:
                                {
                                    var logString = $"Upsert vote type is not valid. Vote id: {articleCommentVote.Id} By Who: {articleCommentVote.ByWho}";
                                    throw new ArticleCommentVoteException(logString);
                                }
                            case VoteType.Like:
                                {
                                    await _articleCommentsVotesManager.UpdateArticleCommentVoteAsync(articleCommentVote.Id, VoteType.Dislike);
                                }
                                break;
                            case VoteType.Dislike:
                                {
                                    await _articleCommentsVotesManager.UpdateArticleCommentVoteAsync(articleCommentVote.Id, VoteType.Like);
                                }
                                break;
                            default:
                                {
                                    var logString = $"Upsert vote type is not implemented. Vote id: {articleCommentVote.Id} By Who: {articleCommentVote.ByWho}";
                                    throw new ArgumentOutOfRangeException(logString);
                                }
                        }
                    }
                }

                //todo: Count Article Comment Votes Difference by aggregation
                var votingPoints = await _articleCommentsVotesManager.CountArticleCommentVotesAsync(articleIdObj, commentIdObj, VoteType.Like) - await _articleCommentsVotesManager.CountArticleCommentVotesAsync(articleIdObj, commentIdObj, VoteType.Dislike);
                return Ok(votingPoints);
            }
            catch (ArticleCommentVoteException e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(e);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        
        
        [HttpGet]
        [Authorize]
        public async Task<long> CountArticleCommentsByArticleIdAsync(string articleId)
        {
            return await _articlesCommentsManager.CountArticleCommentsByArticleIdAsync(articleId);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticleCommentAsync(string articleId, [FromBody] ArticleCommentModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                var articleComment = new MongoDbArticleComment
                {
                    Text = model.Text,
                    ByWho = HttpContext.User.Identity.Name,
                    ByWhoNormalized = _keyNormalizer.Normalize(HttpContext.User.Identity.Name),
                    ReplyTo = model.ReplyTo == null ? ObjectId.Empty : ObjectId.Parse(model.ReplyTo),
                    CreatedAt = DateTime.UtcNow
                };

                await _articlesCommentsManager.CreateArticleComment(articleId, articleComment);

                var comment = await _articlesCommentsManager.FindArticleCommentById(articleId, articleComment.Id);

                return Ok(new ArticleCommentModel
                {
                    Id = comment.Id.ToString(),
                    Text = comment.Text,
                    ReplyTo = comment.ReplyTo == ObjectId.Empty ? null : comment.ReplyTo.ToString(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = comment.CreatedAt.Humanize()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Ok(new ErrorModel
                {
                    ErrorMessage = e.Message,
                    ErrorStatusCode = ErrorStatusCode.InternalServerError
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> FindArticleCommentsByArticleIdAsync(string articleId)
        {
            var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

            var models = new List<ArticleCommentModel>();

            var objectId = ObjectId.Parse(articleId);

            var articleComments = await _articlesCommentsManager.FindArticleCommentsByArticleId(articleId);

            foreach (var articleComment in articleComments)
            {
                var likes = await _articleCommentsVotesManager.CountArticleCommentVotesAsync(objectId, articleComment.Id, VoteType.Like);
                var dislikes = await _articleCommentsVotesManager.CountArticleCommentVotesAsync(objectId, articleComment.Id, VoteType.Dislike);
                models.Add(new ArticleCommentModel
                {
                    Id = articleComment.Id.ToString(),
                    Text = articleComment.Text,
                    ReplyTo = articleComment.ReplyTo == ObjectId.Empty ? null : articleComment.ReplyTo.ToString(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = articleComment.CreatedAt.Humanize(),
                    VotingPoints = likes - dislikes
                });
            }

            return PartialView("GetArticleCommentsPartial", models);
        }
        
        

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProfileDescriptionAsync()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                var artcilesCount = await _articlesManager.CountArticlesByWhoNormalizedAsync(HttpContext.User.Identity.Name);

                return Ok(new ProfileDescription
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Headline = "",
                    ArticlesCount = artcilesCount,
                    FollowingCount = 0,
                    SocialLinks = new List<SocialLink>()
                });
            }
            catch (ProfileDescriptionException e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(e);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }
    }
}