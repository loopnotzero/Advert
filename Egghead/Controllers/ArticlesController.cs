using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
using Egghead.Managers;
using Egghead.Models.Articles;
using Egghead.Models.Errors;
using Egghead.MongoDbStorage.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Egghead.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ILogger _logger;

        private readonly ArticlesManager<MongoDbArticle> _articlesManager;       
        private readonly ArticlesLikesManager<MongoDbArticleLike> _articlesLikesManager;
        private readonly ArticlesViewCountManager<MongoDbArticleViewCount> _articlesViewCountManager;

        public ArticlesController(ArticlesManager<MongoDbArticle> articlesManager, ArticlesLikesManager<MongoDbArticleLike> articlesLikesManager, ArticlesViewCountManager<MongoDbArticleViewCount> articlesViewCountManager, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _articlesManager = articlesManager;
            _articlesLikesManager = articlesLikesManager;
            _articlesViewCountManager = articlesViewCountManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
       
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Like(string articleId)
        {
            try
            {                            
                await _articlesLikesManager.AddArticleLikeAsync(new MongoDbArticleLike
                {
                    ByWhom = HttpContext.User.Identity.Name,
                    ArticleId = articleId,
                    LikeType = LikeType.Like,
                    CreatedAt = DateTime.UtcNow,
                });

                var result = await _articlesLikesManager.CountArticlesLikesByArticleIdAsync(articleId);

                return Ok(result);
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
        public async Task<IActionResult> Dislike(string articleId)
        {
            try
            {
                await _articlesLikesManager.AddArticleLikeAsync(new MongoDbArticleLike
                {
                    ByWhom = HttpContext.User.Identity.Name,
                    ArticleId = articleId,
                    LikeType = LikeType.Dislike,
                    CreatedAt = DateTime.UtcNow,
                });
                
                var result = await _articlesLikesManager.CountArticlesDislikesByArticleIdAsync(articleId);

                return Ok(result);
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
        public async Task<IActionResult> GetArticles()
        {
            try
            {
                var articles = new List<Article>();

                foreach (var article in await _articlesManager.GetArticles())
                {
                    var likes = await _articlesLikesManager.CountArticlesLikesByArticleIdAsync(article.Id);
                    var dislikes = await _articlesLikesManager.CountArticlesDislikesByArticleIdAsync(article.Id);
                    var viewCount = await _articlesViewCountManager.CountArticlesViewCountByArticleIdAsync(article.Id);
                                       
                    articles.Add(new Article
                    {
                        Id = article.Id,
                        Title = article.Title,
                        Text = article.Text,
                        CreatedAt = article.CreatedAt,
                        Likes = likes,
                        Dislikes = dislikes,
                        ViewCount = viewCount,
                        CommentsCount = 0
                    });
                }

                return PartialView("ArticlesPreviewPartial", articles);
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
        public async Task<IActionResult> CreateArticle([FromBody] Article article)
        {
            try
            {
                var newEntity = new MongoDbArticle
                {
                    Title = article.Title,
                    NormalizedTitle = article.Title.ToUpper(),
                    Text = article.Text,
                    CreatedAt = DateTime.UtcNow,
                    ReleaseType = ReleaseType.PreModeration
                };

                await _articlesManager.CreateArticleAsync(newEntity);

                var entity = await _articlesManager.FindArticleByIdAsync(newEntity.Id);

                return PartialView("ArticlesPreviewPartial", new List<Article>
                {
                    new Article
                    {
                        Id = entity.Id,
                        Title = entity.Title,
                        Text = entity.Text,
                        CreatedAt = entity.CreatedAt,
                    }
                });
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

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteArticleById(string objectId)
        {
            try
            {
                var articles = await _articlesManager.GetArticles();
            
                foreach (var article in articles)
                {
                    await _articlesManager.DeleteArticleByIdAsync(article.Id);
                }
            
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
            throw new NotImplementedException();
        }
        
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UdpateArticleById(string objectId, [FromBody] Article article)
        {
            try
            {
                await _articlesManager.UpdateArticleByIdAsync(objectId, new MongoDbArticle
                {
                    Title = article.Title,
                    NormalizedTitle = article.Title.ToUpper(),
                    Text =  article.Text,
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
        public async Task<IActionResult> UdpateArticleByTitle(string title, [FromBody] Article article)
        {
            try
            {
                await _articlesManager.UpdateArticleByTitleAsync(title, new MongoDbArticle
                {
                    Title = article.Title,
                    NormalizedTitle = article.Title.ToUpper(),
                    Text = article.Text,
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
    }
}