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

        public ArticlesController(ArticlesManager<MongoDbArticle> articlesManager, ArticlesLikesManager<MongoDbArticleLike> articlesLikesManager, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _articlesManager = articlesManager;
            _articlesLikesManager = articlesLikesManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
       
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Like(string byWhom, string articleId)
        {
            try
            {
                await _articlesLikesManager.AddLikeAsync(new MongoDbArticleLike
                {
                    ByWhom = byWhom,
                    ArticleId = articleId,
                    CreatedAt = DateTime.UtcNow,
                    LikeType = LikeType.Like
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

        [HttpGet]
        public async Task<IActionResult> Unlike(string byWhom, string articleId)
        {
            try
            {
                await _articlesLikesManager.AddLikeAsync(new MongoDbArticleLike
                {
                    ByWhom = byWhom,
                    ArticleId = articleId,
                    CreatedAt = DateTime.UtcNow,
                    LikeType = LikeType.UnLike
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
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetArticles()
        {
            try
            {
                var articles = await _articlesManager.GetArticles();
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
                var mongoDbArticle = new MongoDbArticle
                {
                    Title = article.Title,
                    NormalizedTitle = article.Title.ToUpper(),
                    Text = article.Text,
                    CreatedAt = DateTime.UtcNow,
                    ReleaseType = ReleaseType.PreModeration
                };
                
                await _articlesManager.CreateArticleAsync(mongoDbArticle);

                var result = await _articlesManager.FindArticleByIdAsync(mongoDbArticle.Id);

                return PartialView("ArticlesPreviewPartial", new List<MongoDbArticle>
                {
                    result
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