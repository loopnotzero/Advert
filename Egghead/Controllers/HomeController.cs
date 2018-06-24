using System.Linq;
using System.Threading.Tasks;
using Egghead.Managers;
using Egghead.Models;
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
        public async Task<IActionResult> GetSubjects()
        {
            var subjects = await _articlesManager.GetSubjects();

            return Ok(subjects.Select(x => new ArticleModel
            {
                Title = x.Title,
                Text = x.Text,
                CreatedAt = x.CreatedAt,
                ReleaseType = x.ReleaseType
            }));
        }
    }
}