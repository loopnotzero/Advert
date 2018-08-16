using System;
using System.Collections.Generic;
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
        private readonly ArticleCommentsLikesManager<MongoDbArticleCommentVote> _articleCommentsLikesManager;

        public ArticlesController(UserManager<MongoDbUser> userManager,
            ArticlesManager<MongoDbArticle> articlesManager,
            ArticlesLikesManager<MongoDbArticleVote> articlesVotesManager,
            ArticlesCommentsManager<MongoDbArticleComment> articlesCommentsManager,
            ArticlesViewCountManager<MongoDbArticleViewCount> articlesViewCountManager,
            ArticleCommentsLikesManager<MongoDbArticleCommentVote> articleCommentsLikesManager, ILoggerFactory loggerFactory
        )
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _userManager = userManager;
            _articlesManager = articlesManager;
            _articlesVotesManager = articlesVotesManager;
            _articlesCommentsManager = articlesCommentsManager;
            _articlesViewCountManager = articlesViewCountManager;
            _articleCommentsLikesManager = articleCommentsLikesManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult WriteArticle()
        {
            return View();
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult GetArticlesPreview()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetArticleContent(string articleId)
        {
            try
            {
                await _articlesViewCountManager.CreateArticleViewCountAsync(new MongoDbArticleViewCount
                {
                    ByWho = HttpContext.User.Identity.Name,
                    ByWhoNormalized = HttpContext.User.Identity.Name.ToUpper(),
                    ArticleId = articleId,
                    CreatedAt = DateTime.UtcNow
                });

                return View();
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
                var articles = await _articlesManager.GetArticlesAsync(20);
                var articlesPreview = new List<ArticlePreviewModel>();

                foreach (var article in articles)
                {
                    articlesPreview.Add(new ArticlePreviewModel
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

                return PartialView("GetArticlesPreviewPartial", articlesPreview);
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
        public async Task<IActionResult> CreateArticle([FromBody] ArticlePreviewModel articlePreview)
        {
            try
            {
                var newArticle = new MongoDbArticle
                {
                    Title = articlePreview.Title,
                    NormalizedTitle = articlePreview.Title.ToUpper(),
                    Text = articlePreview.Text,
                    CreatedAt = DateTime.UtcNow,
                    ReleaseType = ReleaseType.PreModeration,
                };

                await _articlesManager.CreateArticleAsync(newArticle);
                
                //todo: Redirect to articles preview

                return Ok();

//                var result = await _articlesManager.FindArticleByIdAsync(newArticle.Id);
//
//                return PartialView("GetArticlesPreviewPartial", new List<ArticlePreviewModel>
//                {
//                    new ArticlePreviewModel
//                    {
//                        Id = result.Id,
//                        Title = result.Title,
//                        Text = result.Text,
//                        CreatedAt = result.CreatedAt,
//                    }
//                });
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
        public async Task<IActionResult> UdpateArticleById(string articleId, [FromBody] ArticlePreviewModel articlePreview)
        {
            try
            {
                await _articlesManager.UpdateArticleByIdAsync(articleId, new MongoDbArticle
                {
                    Title = articlePreview.Title,
                    NormalizedTitle = articlePreview.Title.ToUpper(),
                    Text = articlePreview.Text,
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
        public async Task<IActionResult> UdpateArticleByTitle(string articleTitle, [FromBody] ArticlePreviewModel articlePreview)
        {
            try
            {
                await _articlesManager.UpdateArticleByTitleAsync(articleTitle, new MongoDbArticle
                {
                    Title = articlePreview.Title,
                    NormalizedTitle = articlePreview.Title.ToUpper(),
                    Text = articlePreview.Text,
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
                    await _articlesVotesManager.CreateArticleVote(new MongoDbArticleVote
                    {
                        ArticleId = articleId,
                        VoteType = model.VoteType,
                        CreatedAt = DateTime.UtcNow,
                        ByWho = HttpContext.User.Identity.Name,
                        ByWhoNormalized = HttpContext.User.Identity.Name.ToUpper(),
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
                var newArticleComment = new MongoDbArticleComment
                {
                    Text = model.Text,
                    ByWho = HttpContext.User.Identity.Name,
                    ByWhoNormalized = HttpContext.User.Identity.Name.ToUpper(),
                    ReplyTo = model.ReplyTo,
                    CreatedAt = DateTime.UtcNow
                };

                var operationReslt = await _articlesCommentsManager.CreateArticleComment(articleId, newArticleComment);

                var articleComment = await _articlesCommentsManager.FindArticleCommentById(articleId, newArticleComment.Id);

                return Ok(new ArticleCommentModel
                {
                    Id = articleComment.Id,
                    Text = articleComment.Text,
                    ReplyTo = articleComment.ReplyTo,
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
            var articleComments = await _articlesCommentsManager.FindArticleCommentsByArticleId(articleId);

            var articleCommentModels = new List<ArticleCommentModel>();

            foreach (var articleComment in articleComments)
            {
                var likes = await _articleCommentsLikesManager.CountArticleCommentLikesByArticleCommentIdAsync(articleId, articleComment.Id);
                var dislikes = await _articleCommentsLikesManager.CountArticleCommentDislikesByArticleCommentIdAsync(articleId, articleComment.Id);
                articleCommentModels.Add(new ArticleCommentModel
                {
                    Id = articleComment.Id,
                    Text = articleComment.Text,
                    ReplyTo = articleComment.ReplyTo,
                    CreatedAt = articleComment.CreatedAt,
                    VotingPoints = likes - dislikes
                });
            }

            return PartialView("GetArticleCommentsPartial", articleCommentModels);
        }
    }
}