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
        private readonly ILogger _logger;

        private readonly UserManager<MongoDbUser> _userManager;
        private readonly ArticlesManager<MongoDbArticle> _articlesManager;       
        private readonly ArticlesLikesManager<MongoDbArticleLike> _articlesLikesManager;
        private readonly ArticlesCommentsManager<MongoDbArticleComment> _articlesCommentsManager;
        private readonly ArticlesViewCountManager<MongoDbArticleViewCount> _articlesViewCountManager;

        public ArticlesController(UserManager<MongoDbUser> userManager, ArticlesManager<MongoDbArticle> articlesManager, ArticlesCommentsManager<MongoDbArticleComment> articlesCommentsManager, ArticlesLikesManager<MongoDbArticleLike> articlesLikesManager, ArticlesViewCountManager<MongoDbArticleViewCount> articlesViewCountManager, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _userManager = userManager;
            _articlesManager = articlesManager;
            _articlesLikesManager = articlesLikesManager;
            _articlesCommentsManager = articlesCommentsManager;
            _articlesViewCountManager = articlesViewCountManager;
        }
       
        [HttpGet]
        [Authorize]
        public IActionResult ArticleContent()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult ArticlesPreview()
        {
            return View();
        }
                      
        [HttpGet]
        [Authorize]
        public IActionResult ArticlePublication()
        {
            return View();
        }
             
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetArticlesPreview(int articlesCount)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                
                var articles = new List<ArticlePreviewModel>();
                
                foreach (var article in await _articlesManager.GetArticlesAsync(articlesCount))
                {
                    var articleLikes = await _articlesLikesManager.CountArticleLikesByArticleIdAsync(article.Id);
                    var articleDislikes = await _articlesLikesManager.CountArticleDislikesByArticleIdAsync(article.Id);
                    var articleViewCount = await _articlesViewCountManager.CountArticleViewCountByArticleIdAsync(article.Id);
                    var articleCommentsCount = await _articlesCommentsManager.CountArticleCommentsByArticleId(article.Id);
               
                    articles.Add(new ArticlePreviewModel
                    {
                        Id = article.Id,
                        Title = article.Title,
                        Text = article.Text.Substring(0, 1000),
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CreatedAt = article.CreatedAt,
                        Likes = articleLikes,
                        Dislikes = articleDislikes,
                        ViewCount = articleViewCount,
                        CommentsCount = articleCommentsCount,
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SetArticleLike(string articleId)
        {
            try
            {         
                var article = await _articlesLikesManager.FindArticleLikesByArticleIdAsync(articleId);

                if (article == null)
                {
                    await _articlesLikesManager.SetArticleLikeAsync(new MongoDbArticleLike
                    {
                        ArticleId = articleId,
                        LikeType = LikeType.Like,
                        CreatedAt = DateTime.UtcNow,
                        ByWho = HttpContext.User.Identity.Name,
                        ByWhoNormalized = HttpContext.User.Identity.Name.ToUpper(),
                    });
                }

                var result = await _articlesLikesManager.CountArticleLikesByArticleIdAsync(articleId);

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
        public async Task<IActionResult> SetArticleDislike(string articleId)
        {
            try
            {
                var article = await _articlesLikesManager.FindArticleDislikesByArticleIdAsync(articleId);

                if (article == null)
                {
                    await _articlesLikesManager.SetArticleLikeAsync(new MongoDbArticleLike
                    {
                        ArticleId = articleId,
                        LikeType = LikeType.Dislike,
                        CreatedAt = DateTime.UtcNow,
                        ByWho = HttpContext.User.Identity.Name,
                        ByWhoNormalized = HttpContext.User.Identity.Name.ToUpper(),
                    });
                }

                var result = await _articlesLikesManager.CountArticleDislikesByArticleIdAsync(articleId);

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
        public async Task<IActionResult> SetArticleViewCount(string articleId)
        {
            try
            {
                await _articlesViewCountManager.SetArticleViewCountAsync(new MongoDbArticleViewCount
                {                                     
                    ByWho = HttpContext.User.Identity.Name,
                    ByWhoNormalized = HttpContext.User.Identity.Name.ToUpper(),
                    ArticleId = articleId,
                    CreatedAt = DateTime.UtcNow
                });

                var result = await _articlesViewCountManager.CountArticleViewCountByArticleIdAsync(articleId);

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
               
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticle([FromBody] ArticlePreviewModel article)
        {
            try
            {
                var newArticle = new MongoDbArticle
                {
                    Title = article.Title,
                    NormalizedTitle = article.Title.ToUpper(),
                    Text = article.Text,
                    CreatedAt = DateTime.UtcNow,
                    ReleaseType = ReleaseType.PreModeration,                  
                };

                await _articlesManager.CreateArticleAsync(newArticle);

                var result = await _articlesManager.FindArticleByIdAsync(newArticle.Id);

                return PartialView("ArticlesPreviewPartial", new List<ArticlePreviewModel>
                {
                    new ArticlePreviewModel
                    {
                        Id = result.Id,
                        Title = result.Title,
                        Text = result.Text,
                        CreatedAt = result.CreatedAt,
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
        public async Task<IActionResult> DeleteArticleByTitle(string articleTitle)
        {
            await _articlesManager.DeleteArticleByTitleAsync(articleTitle);
            return Ok();
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UdpateArticleById(string articleId, [FromBody] ArticlePreviewModel article)
        {
            try
            {
                await _articlesManager.UpdateArticleByIdAsync(articleId, new MongoDbArticle
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
        public async Task<IActionResult> UdpateArticleByTitle(string articleTitle, [FromBody] ArticlePreviewModel article)
        {
            try
            {
                await _articlesManager.UpdateArticleByTitleAsync(articleTitle, new MongoDbArticle
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
                 
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetArticleCommentsByArticleId(string articleId)
        {
            throw new NotImplementedException();
        }                              
    }
}