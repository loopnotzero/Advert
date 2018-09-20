using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Egghead.Common.Articles;
using Egghead.Common.Profiles;
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
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Egghead.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ILogger _logger;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly UserManager<MongoDbUser> _userManager;
        private readonly ProfilesManager<MongoDbProfile> _profilesManager;
        private readonly ArticlesManager<MongoDbArticle> _articlesManager;
        private readonly ArticlesLikesManager<MongoDbArticleVote> _articlesVotesManager;
        private readonly ArticlesCommentsManager<MongoDbArticleComment> _articlesCommentsManager;
        private readonly ArticlesViewCountManager<MongoDbArticleViewsCount> _articlesViewCountManager;
        private readonly ArticleCommentsVotesManager<MongoDbArticleCommentVote> _articleCommentsVotesManager;
           
        public ArticlesController(UserManager<MongoDbUser> userManager, ProfilesManager<MongoDbProfile> profilesManager, ArticlesManager<MongoDbArticle> articlesManager, ArticlesLikesManager<MongoDbArticleVote> articlesVotesManager, ArticlesCommentsManager<MongoDbArticleComment> articlesCommentsManager, ArticlesViewCountManager<MongoDbArticleViewsCount> articlesViewCountManager, ArticleCommentsVotesManager<MongoDbArticleCommentVote> articleCommentsVotesManager, ILoggerFactory loggerFactory, ILookupNormalizer keyNormalizer)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _keyNormalizer = keyNormalizer;
            _userManager = userManager;
            _profilesManager = profilesManager;
            _articlesManager = articlesManager;
            _articlesVotesManager = articlesVotesManager;
            _articlesCommentsManager = articlesCommentsManager;
            _articlesViewCountManager = articlesViewCountManager;
            _articleCommentsVotesManager = articleCommentsVotesManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult ComposeArticle()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetArticleContent(string articleId)
        {
            try
            {
                var opertaionResult = await _articlesViewCountManager.CreateArticleViewCountAsync(new MongoDbArticleViewsCount
                {
                    ArticleId = ObjectId.Parse(articleId),
                    CreatedAt = DateTime.UtcNow
                });

                if (!opertaionResult.Succeeded)
                {
                    //todo: Handle error
                }
                 
                var article = await _articlesManager.FindArticleByIdAsync(ObjectId.Parse(articleId));

                return View(new ArticleModel
                {
                    Id = article.Id.ToString(),
                    Title = article.Title,
                    Text = article.Text,
                    LikesCount = article.LikesCount,
                    DislikesCount = article.DislikesCount,
                    ViewsCount = article.ViewsCount,
                    CommentsCount = article.CommentsCount,
                    CreatedAt = article.CreatedAt.Humanize()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Ok();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetArticlesPreview()
        {
            try
            {
                var articles = new List<ArticleModel>();
                
                foreach (var article in await _articlesManager.FindArticlesAsync(50))
                {
                    articles.Add(new ArticleModel
                    {
                        Id = article.Id.ToString(),
                        Title = article.Title,
                        Text = (article.Text.Length > 1000 ? article.Text.Substring(0, 1000) : article.Text) + "...",
                        LikesCount = article.LikesCount,
                        DislikesCount = article.DislikesCount,
                        ViewsCount = article.ViewsCount,
                        CommentsCount = article.CommentsCount,
                        CreatedAt = article.CreatedAt.Humanize(),
                    });
                }

                return View(articles);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }
  
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PublishArticleAsync([FromBody] PublishArticleModel model)
        {
            try
            {
                var operationResult = await _articlesManager.CreateArticleAsync(new MongoDbArticle
                {                   
                    Title = model.Title,
                    NormalizedTitle = NormalizeKey(model.Title),
                    Text = model.Text,                
                    CreatedAt = DateTime.UtcNow,
                    ReleaseType = ReleaseType.PreModeration,
                });

                if (!operationResult.Succeeded)
                {
                    //todo: Handle error
                }

                return Ok(new
                {
                    returnUrl = "/"
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

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UdpateArticleByIdAsync(string articleId, [FromBody] ArticleModel model)
        {
            try
            {
                await _articlesManager.UpdateArticleByIdAsync(ObjectId.Parse(articleId), new MongoDbArticle
                {
                    Title = model.Title,
                    NormalizedTitle = _keyNormalizer.Normalize(model.Title),
                    Text = model.Text,
                    ChangedAt = DateTime.UtcNow
                });

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
        public async Task<IActionResult> UdpateArticleByTitleAsync(string title, [FromBody] ArticleModel model)
        {
            try
            {
                await _articlesManager.UpdateArticleByTitleAsync(title, new MongoDbArticle
                {
                    Title = model.Title,
                    NormalizedTitle = _keyNormalizer.Normalize(model.Title),
                    Text = model.Text,
                    ChangedAt = DateTime.UtcNow,
                });

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
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
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
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
                var articleObjectId = ObjectId.Parse(articleId);

//                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var articleVote = await _articlesVotesManager.FindArticleVoteByProfileIdAsync(articleObjectId, model.VoteType, ObjectId.Empty);

                if (articleVote == null)
                {
                    await _articlesVotesManager.CreateArticleVoteAsync(new MongoDbArticleVote
                    {            
                        ArticleId = articleObjectId,
                        ProfileId = ObjectId.Empty,
                        VoteType = model.VoteType,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                var articleVotes = await _articlesVotesManager.CountArticleVotesAsync(articleObjectId, model.VoteType);

                return Ok(articleVotes);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpsertArticleCommentVoteAsync(string articleId, [FromBody] ArticleCommentVoteModel model)
        {
            try
            {
                if (model.VoteType == VoteType.None)
                {
                    var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                    var logString = $"Upsert vote type is not valid. Article id: {articleId} By Who: {HttpContext.User.Identity.Name}";
                    throw new ArticleCommentVoteException(logString);
                }

                var articleCommentVote = await _articleCommentsVotesManager.FindArticleCommentVoteAsync(ObjectId.Parse(articleId), ObjectId.Parse(model.CommentId));

                if (articleCommentVote == null)
                {
//                    var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                    articleCommentVote = new MongoDbArticleCommentVote
                    {                  
                        ArticleId = ObjectId.Parse(articleId),
                        CommentId = ObjectId.Parse(model.CommentId),
                        ProfileId = ObjectId.Empty,
                        VoteType = model.VoteType,
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    await _articleCommentsVotesManager.CreateArticleCommentVoteAsync(articleCommentVote);   
                    
                    var votingPoints = await _articleCommentsVotesManager.CountArticleCommentVotesAsync(articleCommentVote.ArticleId, articleCommentVote.CommentId, VoteType.Like) - await _articleCommentsVotesManager.CountArticleCommentVotesAsync(articleCommentVote.ArticleId, articleCommentVote.CommentId, VoteType.Dislike);

                    return Ok(votingPoints);
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
                                    var logString = $"Upsert vote type is not valid. Vote id: {articleCommentVote.Id} By Who: {articleCommentVote.ProfileId}";
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
                                    var logString = $"Upsert vote type is not implemented. Vote id: {articleCommentVote.Id} By Who: {HttpContext.User.Identity.Name}";
                                    throw new ArgumentOutOfRangeException(logString);
                                }
                        }
                    }

                    var votingPoints = await _articleCommentsVotesManager.CountArticleCommentVotesAsync(articleCommentVote.ArticleId, articleCommentVote.CommentId, VoteType.Like) - await _articleCommentsVotesManager.CountArticleCommentVotesAsync(articleCommentVote.ArticleId, articleCommentVote.CommentId, VoteType.Dislike);

                    return Ok(votingPoints);  
                }     
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
                
//                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);
     
                var articleComment = new MongoDbArticleComment
                {
                    Text = model.Text,         
                    ReplyTo = model.ReplyTo == null ? ObjectId.Empty : ObjectId.Parse(model.ReplyTo),
                    ProfileId = ObjectId.Empty,
                    CreatedAt = DateTime.UtcNow
                };

                await _articlesCommentsManager.CreateArticleComment(articleId, articleComment);

                var comment = await _articlesCommentsManager.FindArticleCommentById(articleId, articleComment.Id);

                return Ok(new ArticleCommentModel
                {
                    Id = comment.Id.ToString(),
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

//                var profile = await _profilesManager.FindProfileByNormalizedEmailAsync(HttpContext.User.Identity.Name);

                var artcilesCount = await _articlesManager.CountArticlesByProfileIdAsync(ObjectId.Empty);

                return Ok(new ProfileDescription
                {
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
        
        private string NormalizeKey(string key)
        {
            return _keyNormalizer != null ? _keyNormalizer.Normalize(key) : key;
        }
    }
}