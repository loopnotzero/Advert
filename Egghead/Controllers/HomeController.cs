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

        private readonly SubjectsManager<MongoDbSubject> _subjectsManager;

        public HomeController(SubjectsManager<MongoDbSubject> subjectsManager, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AccountController>();
            _subjectsManager = subjectsManager;
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
            var subjects = await _subjectsManager.GetSubjects();

            return Ok(subjects.Select(x => new SubjectModel
            {
                Title = x.Title,
                Text = x.Text,
                CreatedAt = x.CreatedAt,
                ReleaseType = x.ReleaseType
            }));
        }
    }
}