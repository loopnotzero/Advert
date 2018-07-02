using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Common.Articles;
using Egghead.Managers;
using Egghead.Models.Articles;
using Egghead.Models.Errors;
using Egghead.MongoDbStorage.Entities;
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
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Article(string id)
        {
            var article = await _articlesManager.FindArticleByIdAsync(id);

            return View(new Article
            {
                Id = article.Id
            });
        }
            
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetArticles()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                
                var articles = new List<ArticlePreview>();
                
                foreach (var article in await _articlesManager.GetArticles())
                {
                    var likes = await _articlesLikesManager.CountArticlesLikesByArticleIdAsync(article.Id);
                    var dislikes = await _articlesLikesManager.CountArticlesDislikesByArticleIdAsync(article.Id);
                    var viewCount = await _articlesViewCountManager.CountArticlesViewCountByArticleIdAsync(article.Id);
                    var commentsCount = await _articlesCommentsManager.CountArticlesCommentsByArticleId(article.Id);
               
                    articles.Add(new ArticlePreview
                    {
                        Id = article.Id,
                        Title = article.Title,
                        Text = article.Text,
                        Author = $"{user.FirstName} {user.LastName}",
                        CreatedAt = article.CreatedAt,
                        Likes = likes,
                        Dislikes = dislikes,
                        ViewCount = viewCount,
                        CommentsCount = commentsCount,
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
        public async Task<IActionResult> CreateArticle([FromBody] ArticlePreview article)
        {
            try
            {
                var newArticle = new MongoDbArticle
                {
                    Title = article.Title,
                    NormalizedTitle = article.Title.ToUpper(),
                    Text = article.Text,
                    CreatedAt = DateTime.UtcNow,
                    ReleaseType = ReleaseType.PreModeration
                };

                await _articlesManager.CreateArticleAsync(newArticle);

                var result = await _articlesManager.FindArticleByIdAsync(newArticle.Id);

                return PartialView("ArticlesPreviewPartial", new List<ArticlePreview>
                {
                    new ArticlePreview
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
        public async Task<IActionResult> UdpateArticleById(string objectId, [FromBody] ArticlePreview article)
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
        public async Task<IActionResult> UdpateArticleByTitle(string title, [FromBody] ArticlePreview article)
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
       
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddArticleView(string articleId)
        {
            try
            {
                await _articlesViewCountManager.AddArticlesViewAsync(new MongoDbArticleViewCount
                {
                    ByWhom = HttpContext.User.Identity.Name,
                    ArticleId = articleId,
                    AddedAt = DateTime.UtcNow
                });

                var result = await _articlesViewCountManager.CountArticlesViewCountByArticleIdAsync(articleId);

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
        public async Task<IActionResult> AddArticleLike(string articleId)
        {
            try
            {         
                var article = await _articlesLikesManager.FindArticlesLikesByArticleIdAsync(articleId);

                if (article == null)
                {
                    await _articlesLikesManager.AddArticleLikeAsync(new MongoDbArticleLike
                    {
                        ByWhom = HttpContext.User.Identity.Name,
                        ArticleId = articleId,
                        LikeType = LikeType.Like,
                        CreatedAt = DateTime.UtcNow,
                    });
                }

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
        [Authorize]
        public async Task<IActionResult> AddArticleDislike(string articleId)
        {
            try
            {
                var article = await _articlesLikesManager.FindArticlesDislikesByArticleIdAsync(articleId);

                if (article == null)
                {
                    await _articlesLikesManager.AddArticleLikeAsync(new MongoDbArticleLike
                    {
                        ByWhom = HttpContext.User.Identity.Name,
                        ArticleId = articleId,
                        LikeType = LikeType.Dislike,
                        CreatedAt = DateTime.UtcNow,
                    });
                }

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
    }
}