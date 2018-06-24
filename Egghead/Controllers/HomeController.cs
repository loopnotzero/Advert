using System;
using System.Linq;
using System.Threading.Tasks;
using Egghead.Common;
using Egghead.Managers;
using Egghead.Models.Articles;
using Egghead.Models.Errors;
using Egghead.MongoDbStorage.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Egghead.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;

        private readonly ArticlesManager<MongoDbArticle> _articlesManager;

        public HomeController(ArticlesManager<MongoDbArticle> articlesManager, ILoggerFactory loggerFactory)
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
            var subjects = await _articlesManager.GetArticles();

            return Ok(subjects.Select(x => new Article
            {
                Title = x.Title,
                Text = x.Text,
            }));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticle article)
        {
            try
            {
                var result = await _articlesManager.CreateArticleAsync(new MongoDbArticle
                {
                    Title = article.Title,
                    NormalizedTitle = article.Title.ToUpper(),
                    Text = article.Text
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
        public async Task<IActionResult> UdpateArticleById(string id)
        {
            var article = await _articlesManager.FindArticleByIdAsync(id);           
            await _articlesManager.UpdateArticleAsync(article);
            return Ok();
        }
        
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UdpateArticleByTitle(string title)
        {
            var article = await _articlesManager.FindArticleByTitleAsync(title);           
            await _articlesManager.UpdateArticleAsync(article);
            return Ok();
        }
    }
}