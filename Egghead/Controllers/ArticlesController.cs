using System;
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

        public ArticlesController(ArticlesManager<MongoDbArticle> articlesManager, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _articlesManager = articlesManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetArticles()
        {
            var articles = await _articlesManager.GetArticles();
            return PartialView("ArticlePartial", articles);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticle([FromBody] Article article)
        {
            try
            {
                var result = await _articlesManager.CreateArticleAsync(new MongoDbArticle
                {
                    Title = article.Title,
                    NormalizedTitle = article.Title.ToUpper(),
                    Text = article.Text,
                    CreatedAt = DateTime.UtcNow,
                    ReleaseType = ReleaseType.PreModeration
                });

                return Ok(new ErrorModel
                {
                    RedirectUrl = "Redirect_To_Article_Page",
                    ErrorStatusCode = ErrorStatusCode.Created,                 
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
        public async Task<IActionResult> DeleteArticleById(string id)
        {
            throw new NotImplementedException();
        }
        
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteArticleByTitle(string title)
        {
            throw new NotImplementedException();
        }
        
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UdpateArticleById(string id, [FromBody] Article article)
        {
            await _articlesManager.UpdateArticleAsync(new MongoDbArticle
            {
                Id = id,
                Title = article.Title,
                NormalizedTitle = article.Title.ToUpper(),
                Text =  article.Text,
                ChangedAt = DateTime.UtcNow
            });
            return Ok();
        }
        
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UdpateArticleByTitle(string title, [FromBody] Article article)
        {
            await _articlesManager.UpdateArticleAsync(new MongoDbArticle
            {
                Id = article.Id,
                Title = title,
                NormalizedTitle = article.Title.ToUpper(),
                Text =  article.Text,
                ChangedAt = DateTime.UtcNow
            });
            return Ok();
        }
    }
}