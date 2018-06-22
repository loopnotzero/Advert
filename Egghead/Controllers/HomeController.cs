using System;
using System.Linq;
using System.Threading.Tasks;
using Egghead.Managers;
using Egghead.Models;
using Egghead.MongoDbStorage.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Egghead.Controllers
{
    public class HomeController : Controller
    {
        private readonly SubjectsManager<MongoDbSubject> _subjectsManager;

        public HomeController(SubjectsManager<MongoDbSubject> subjectsManager)
        {
            _subjectsManager = subjectsManager;
        }
        
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> GetTopics()
        {
            //var topics = await _topicsManager.GetTopics();
            return Ok(null);
        }
    }
}