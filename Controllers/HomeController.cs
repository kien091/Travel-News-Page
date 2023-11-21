using Final_NET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Final_NET.Controllers
{
    public class HomeController : Controller
    {
        private TravelNewsDBContext _dbContext;
        public HomeController(TravelNewsDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("/Home")]
        public IActionResult Home()
        {
            TempData["Id"] = HttpContext.Session.GetString("Id");
            TempData["Username"] = HttpContext.Session.GetString("Username");

            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Login");

            var articles = _dbContext.Articles.ToList();

            return View("Home", articles);
        }

        [Route("/Home/Location")]
        public IActionResult Location()
        {
            TempData["Id"] = HttpContext.Session.GetString("Id");
            TempData["Username"] = HttpContext.Session.GetString("Username");

            var articles = _dbContext.Articles.Where(a => a.Category.Equals("Du lịch")).ToList();

            return View("Home", articles);
        }

        [Route("/Home/Cuisine")]
        public IActionResult Cuisine()
        {
            TempData["Id"] = HttpContext.Session.GetString("Id");
            TempData["Username"] = HttpContext.Session.GetString("Username");

            var articles = _dbContext.Articles.Where(a => a.Category.Equals("Ẩm thực")).ToList();

            return View("Home", articles);
        }

        [Route("/Home/Manual")]
        public IActionResult Manual()
        {
            TempData["Id"] = HttpContext.Session.GetString("Id");
            TempData["Username"] = HttpContext.Session.GetString("Username");

            var articles = _dbContext.Articles.Where(a => a.Category.Equals("Cẩm nang")).ToList();

            return View("Home", articles);
        }

        [Route("/Home/Detail")]
        public IActionResult Detail(string Location, string Category)
        {
            TempData["Id"] = HttpContext.Session.GetString("Id");
            TempData["Username"] = HttpContext.Session.GetString("Username");

            var art = _dbContext.Articles
                .Where(a => a.Category.Equals(Category) && a.Location.Equals(Location))
                .ToList();

            return View("Detail", art);
        }

        [Route("/Home/Reading")]
        public IActionResult Reading(string Location, string Category, string Id)
        {
            TempData["Id"] = HttpContext.Session.GetString("Id");
            TempData["Username"] = HttpContext.Session.GetString("Username");

            var art = _dbContext.Articles
                .Where(a => a.Category.Equals(Category) && a.Location.Equals(Location))
                .ToList();

            TempData["IdArticles"] = Id;

            var article = _dbContext.Articles.Find(Convert.ToInt32(Id));

            if(article != null)
            {
                article.View += 1;
                _dbContext.SaveChanges();
            }

            return View("Reading", art);
        }
    }
}