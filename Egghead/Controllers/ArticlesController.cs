using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
using Egghead.Managers;
using Egghead.Models.Articles;
using Egghead.Models.Errors;
using Egghead.MongoDbStorage.Articles;
using Egghead.MongoDbStorage.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Egghead.Controllers
{
    public class ArticlesController : Controller
    {
        public const int ArticlePreviewMaxLength = 1000;

        private readonly ILogger _logger;
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
            ArticleCommentsVotesManager<MongoDbArticleCommentVote> articleCommentsVotesManager, ILoggerFactory loggerFactory
        )
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
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
        public async Task<IActionResult> GetArticlesPreview()
        {
            try          
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                var articles = new List<MongoDbArticle>();

                foreach (var articleId in await _articlesViewCountManager.FindArticlesPopularOnEgghead(5))
                {
                    articles.Add(await _articlesManager.FindArticleByIdAsync(articleId));
                }
            
                return View(articles.Select(x => new ArticleModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Text = x.Text,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = x.CreatedAt
                }));
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
        public async Task<IActionResult> GetArticleContent(string articleId)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                
                await _articlesViewCountManager.CreateArticleViewCountAsync(new MongoDbArticleViewCount
                {
                    ByWho = HttpContext.User.Identity.Name,
                    ByWhoNormalized = HttpContext.User.Identity.Name.ToUpper(),
                    ArticleId = articleId,
                    CreatedAt = DateTime.UtcNow
                });

                var article = await _articlesManager.FindArticleByIdAsync(articleId);
                
                var model = new ArticleModel
                {
                    Id = article.Id,
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
        public async Task<IActionResult> GetArticlesPreviewPartial()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                var models = new List<ArticlePreviewModel>();
                var articles = await _articlesManager.FindArticlesAsync(20);

                foreach (var article in articles)
                {
                    models.Add(new ArticlePreviewModel
                    {
                        Id = article.Id,
                        Title = article.Title,
                        Text = article.Text.Length > ArticlePreviewMaxLength ? article.Text.Substring(0, 1000) : article.Text,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CreatedAt = article.CreatedAt,
                        Likes = await _articlesVotesManager.CountArticleVotesAsync(article.Id, VoteType.Like),
                        Dislikes = await _articlesVotesManager.CountArticleVotesAsync(article.Id, VoteType.Dislike),
                        ViewCount = await _articlesViewCountManager.CountArticleViewCountAsync(article.Id),
                        CommentsCount = await _articlesCommentsManager.CountArticleCommentsByArticleIdAsync(article.Id),
                    });
                }

                return PartialView("GetArticlesPreviewPartial", models);
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
        public async Task<IActionResult> CreateArticle([FromBody] ArticlePreviewModel model)
        {
            try
            {
                await _articlesManager.CreateArticleAsync(new MongoDbArticle
                {
                    Title = model.Title,
                    NormalizedTitle = model.Title.ToUpper(),
                    Text = model.Text,
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
        public async Task<IActionResult> GetArticleById(string articleId)
        {
            try
            {
                var article = await _articlesManager.FindArticleByIdAsync(articleId);
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
        public async Task<IActionResult> UdpateArticleById(string articleId, [FromBody] ArticlePreviewModel model)
        {
            try
            {
                await _articlesManager.UpdateArticleByIdAsync(articleId, new MongoDbArticle
                {
                    Title = model.Title,
                    NormalizedTitle = model.Title.ToUpper(),
                    Text = model.Text,
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
        public async Task<IActionResult> UdpateArticleByTitle(string articleTitle, [FromBody] ArticlePreviewModel model)
        {
            try
            {
                await _articlesManager.UpdateArticleByTitleAsync(articleTitle, new MongoDbArticle
                {
                    Title = model.Title,
                    NormalizedTitle = model.Title.ToUpper(),
                    Text = model.Text,
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
        public async Task<IActionResult> DeleteArticleById(string articleId)
        {
            try
            {
                await _articlesManager.DeleteArticleByIdAsync(articleId);
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
        public async Task<IActionResult> DeleteArticleByTitle(string title)
        {
            await _articlesManager.DeleteArticleByTitleAsync(title);
            return Ok();
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticleVote(string articleId, [FromBody] ArticleVoteModel model)
        {
            try
            {
                var articleVote = await _articlesVotesManager.FindArticleVoteAsync(articleId, model.VoteType, HttpContext.User.Identity.Name.ToUpper());

                if (articleVote == null)
                {
                    await _articlesVotesManager.CreateArticleVoteAsync(new MongoDbArticleVote
                    {
                        ByWho = HttpContext.User.Identity.Name,
                        ByWhoNormalized = HttpContext.User.Identity.Name.ToUpper(),
                        ArticleId = articleId,
                        VoteType = model.VoteType,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                var articleVotes = await _articlesVotesManager.CountArticleVotesAsync(articleId, model.VoteType);

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
        public async Task<IActionResult> CreateArticleCommentVote(string articleId, [FromBody] ArticleCommentVoteModel model)
        {
            try
            {
                switch (model.VoteType)
                {
                    case VoteType.None:
                        {
                            var logString = $"Couldn't create comment vote for article id: {articleId} comment id: {model.CommentId}";
                            _logger.LogError(logString);
                            throw new Exception(logString);
                        }
                        break;
                    default:
                        {
                            var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name.ToUpper());
                            
                            var articleCommentVote = await _articleCommentsVotesManager.FindArticleCommentVoteAsync(articleId, model.CommentId, model.VoteType, HttpContext.User.Identity.Name.ToUpper());

                            if (articleCommentVote == null)
                            {
                                await _articleCommentsVotesManager.CreateArticleCommentVoteAsync(new MongoDbArticleCommentVote
                                {
                                    ByWho = HttpContext.User.Identity.Name,
                                    ByWhoNormalized = HttpContext.User.Identity.Name.ToUpper(),
                                    ArticleId = articleId,
                                    CommentId = model.CommentId,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    VoteType = model.VoteType,
                                    CreatedAt = DateTime.UtcNow
                                });
                            }

                            var votingPoints = await _articleCommentsVotesManager.CountArticleCommentVotesAsync(articleId, model.CommentId, VoteType.Like) - await _articleCommentsVotesManager.CountArticleCommentVotesAsync(articleId, model.CommentId, VoteType.Dislike);
                            
                            return Ok(votingPoints);
                        }
                }
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
        public async Task<long> CountArticleCommentsByArticleId(string articleId)
        {
            return await _articlesCommentsManager.CountArticleCommentsByArticleIdAsync(articleId);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticleComment(string articleId, [FromBody] ArticleCommentModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                var articleComment = new MongoDbArticleComment
                {
                    Text = model.Text,
                    ByWho = HttpContext.User.Identity.Name,
                    ByWhoNormalized = HttpContext.User.Identity.Name.ToUpper(),
                    ReplyTo = model.ReplyTo,
                    CreatedAt = DateTime.UtcNow
                };

                await _articlesCommentsManager.CreateArticleComment(articleId, articleComment);
       
                return Ok(new ArticleCommentModel
                {
                    Id = articleComment.Id,
                    Text = articleComment.Text,
                    ReplyTo = articleComment.ReplyTo,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    VotingPoints = 0,
                    CreatedAt = articleComment.CreatedAt
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
        public async Task<IActionResult> FindArticleCommentsByArticleId(string articleId)
        {
            var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name.ToUpper());

            var models = new List<ArticleCommentModel>();

            var articleComments = await _articlesCommentsManager.FindArticleCommentsByArticleId(articleId);

            foreach (var articleComment in articleComments)
            {
                var likes = await _articleCommentsVotesManager.CountArticleCommentVotesAsync(articleId, articleComment.Id, VoteType.Like);
                var dislikes = await _articleCommentsVotesManager.CountArticleCommentVotesAsync(articleId, articleComment.Id, VoteType.Dislike);
                models.Add(new ArticleCommentModel
                {
                    Id = articleComment.Id,
                    Text = articleComment.Text,
                    ReplyTo = articleComment.ReplyTo,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = articleComment.CreatedAt,
                    VotingPoints = likes - dislikes
                });
            }

            return PartialView("GetArticleCommentsPartial", models);
        }
    }
}