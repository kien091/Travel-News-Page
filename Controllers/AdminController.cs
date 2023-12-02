using Final_NET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Final_NET.Controllers
{
    public class AdminController : Controller
    {
        private TravelNewsDBContext _dbContext;
        public AdminController(TravelNewsDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("/Admin")]
        public IActionResult Admin()
        {
            TempData["Role"] = HttpContext.Session.GetString("Role");
            TempData["Id"] = HttpContext.Session.GetString("Id");

            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Login");

            TempData["Username"] = HttpContext.Session.GetString("Username");
            List<Account> accounts = _dbContext.Accounts.ToList();
            return View("Admin", accounts);
        }

        [HttpGet]
        [Route("/Location")]
        public IActionResult Location()
        {
            TempData["Role"] = HttpContext.Session.GetString("Role");
            TempData["Id"] = HttpContext.Session.GetString("Id");
            TempData["Username"] = HttpContext.Session.GetString("Username");

            if (HttpContext.Session.GetString("Role").Equals("SuperAdmin"))
            {
                List<Articles> articles = _dbContext.Articles.Where(a => a.Category == "Du lịch").ToList();
                return View("Articles", articles);
            }
            else
            {
                List<Articles> articles = _dbContext.Articles
                    .Where(a => a.Category == "Du lịch" 
                    && a.AccountId == Convert.ToInt32(HttpContext.Session.GetString("Id"))).ToList();
                return View("Articles", articles);
            }
        }

        [HttpGet]
        [Route("/Cuisine")]
        public IActionResult Cuisine()
        {
            TempData["Role"] = HttpContext.Session.GetString("Role");
            TempData["Id"] = HttpContext.Session.GetString("Id");
            TempData["Username"] = HttpContext.Session.GetString("Username");

            if (HttpContext.Session.GetString("Role").Equals("SuperAdmin"))
            {
                List<Articles> articles = _dbContext.Articles.Where(a => a.Category == "Ẩm thực").ToList();
                return View("Articles", articles);
            }
            else
            {
                List<Articles> articles = _dbContext.Articles
                    .Where(a => a.Category == "Ẩm thực"
                    && a.AccountId == Convert.ToInt32(HttpContext.Session.GetString("Id"))).ToList();
                return View("Articles", articles);
            }
        }

        [HttpGet]
        [Route("/Manual")]
        public IActionResult Manual()
        {
            TempData["Role"] = HttpContext.Session.GetString("Role");
            TempData["Id"] = HttpContext.Session.GetString("Id");
            TempData["Username"] = HttpContext.Session.GetString("Username");

            if (HttpContext.Session.GetString("Role").Equals("SuperAdmin"))
            {
                List<Articles> articles = _dbContext.Articles.Where(a => a.Category == "Cẩm nang").ToList();
                return View("Articles", articles);
            }
            else
            {
                List<Articles> articles = _dbContext.Articles
                    .Where(a => a.Category == "Cẩm nang"
                    && a.AccountId == Convert.ToInt32(HttpContext.Session.GetString("Id"))).ToList();
                return View("Articles", articles);
            }
        }

        [Route("/UpdateAccount/{id?}")]
        public IActionResult UpdateAccount(string id)
        {
            TempData["Role"] = HttpContext.Session.GetString("Role");

            return RedirectToAction("Profile", "Profile", new {id = id});
        }


        [Route("/DeleteAccount")]
        public IActionResult DeleteAccount(string id)
        {
            TempData["Role"] = HttpContext.Session.GetString("Role");

            var account = _dbContext.Accounts.Find(Convert.ToInt32(id));

            if(account != null)
            {
                _dbContext.Accounts.Remove(account);
                _dbContext.SaveChanges();
            }

            List<Account> accounts = _dbContext.Accounts.ToList();
            return View("Admin", accounts);
        }

        [HttpPost]
        [Route("/SearchAccount")]
        public IActionResult SearchAccount()
        {
            TempData["Role"] = HttpContext.Session.GetString("Role");
            string search = Request.Form["input-search"];

            var account = _dbContext.Accounts
                .Where(f => f.Id.ToString().Contains(search.ToLower())
                || f.Username.ToLower().Contains(search.ToLower()) 
                || f.Email.ToLower().Contains(search.ToLower())
                || f.Role.ToLower().Contains(search.ToLower()) 
                || (f.Status == 0 && search.ToLower() == "Lock")
                || (f.Status == 1 && search.ToLower() == "Normal"))
                .ToList();

            return View("Admin", account);
        }

        [Route("/Add")]
        public IActionResult Add()
        {
            var articles = new Articles
            {
                Id = 0
            };
            return View("Add", articles);
        }

        [Route("/Save/{id?}")]
        public IActionResult SaveArticle(string id, Articles art) 
        {
            var article = new Articles();
            if(Convert.ToInt32(id) == 0)
            {
                article = new Articles
                {
                    Image = art.Image,
                    Title = art.Title,
                    Content = ReplaceImageLink(art.Content),
                    Category = art.Category,
                    Location = art.Location,
                    AccountId = Convert.ToInt32(HttpContext.Session.GetString("Id"))
                };
                _dbContext.Add(article);
                _dbContext.SaveChanges();

                ViewBag.Success = "Thêm thành công!";
            }
            else
            {
                article = _dbContext.Articles.Find(Convert.ToInt32(id));
                if(article != null)
                {
                    article.Image = art.Image;
                    article.Title = art.Title;
                    article.Date = DateTime.Now;
                    article.Content = art.Content;
                    article.Category = art.Category;
                    article.Location = art.Location;
                    article.View = 0;

                    _dbContext.SaveChanges();

                    ViewBag.Success = "Sửa đổi bài báo thành công!";
                }
            }

            return View("Add", article);
        }

        private string ReplaceImageLink(string input)
        {
            Regex regex = new Regex(@"https://\S+\.(jpg|png|gif|jpeg|webp)");
            string output = regex.Replace(input, @"<div style=""display: flex; align-items: center; justify-content: center; margin: 10px""><img src=""$0"" alt=""~/img/error.jpg""></div>");
            return output;
        }


        [Route("/UpdateArticle/{id?}")]
        public IActionResult UpdateArticle(string id)
        {
            TempData["Role"] = HttpContext.Session.GetString("Role");

            var article = _dbContext.Articles.Find(Convert.ToInt32(id));

            return View("Add", article);
        }


        [Route("/DeleteArticle/{id?}")]
        public IActionResult DeleteArticle(string id)
        {
            TempData["Role"] = HttpContext.Session.GetString("Role");

            var article = _dbContext.Articles.Find(Convert.ToInt32(id));
            int idAccount = Convert.ToInt32(HttpContext.Session.GetString("Id"));

            if (article != null)
            {
                _dbContext.Articles.Remove(article);
                _dbContext.SaveChanges();
            }

            var articles = new List<Articles>();
            if (HttpContext.Session.GetString("Role").Equals("SuperAdmin"))
            {
                articles = _dbContext.Articles.ToList();
            }
            else
            {
                articles = _dbContext.Articles.Where(a => a.AccountId == Convert.ToInt32(idAccount)).ToList();
            }
            return View("Articles", articles);
        }

        [HttpPost]
        [Route("/SearchArticle")]
        public IActionResult SearchArticle()
        {
            TempData["Role"] = HttpContext.Session.GetString("Role");
            string search = Request.Form["input-search"];
            var article = new List<Articles>();
            int id = Convert.ToInt32(HttpContext.Session.GetString("Id"));

            if (HttpContext.Session.GetString("Role").Equals("SuperAdmin"))
            {
                article = _dbContext.Articles
                .Where(f => f.Id.ToString().Contains(search.ToLower())
                || f.Title.ToLower().Contains(search.ToLower())
                || f.Date.ToString().ToLower().Contains(search.ToLower())
                || f.Category.ToLower().Contains(search.ToLower())
                || f.Location.ToLower().Contains(search.ToLower())
                || f.View.ToString().ToLower().Contains(search.ToLower()))
                .ToList();
            }
            else
            {
                article = _dbContext.Articles
                .Where(f => f.Id.ToString().Contains(search.ToLower())
                || f.Title.ToLower().Contains(search.ToLower())
                || f.Date.ToString().ToLower().Contains(search.ToLower())
                || f.Category.ToLower().Contains(search.ToLower())
                || f.Location.ToLower().Contains(search.ToLower())
                || f.View.ToString().ToLower().Contains(search.ToLower())
                && f.AccountId == id)
                .ToList();
            }

            return View("Articles", article);
        }
    }
}
