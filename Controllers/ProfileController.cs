using Final_NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Final_NET.Controllers
{
    public class ProfileController : Controller
    {
        private TravelNewsDBContext _dbContext;
        public ProfileController(TravelNewsDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("/Profile/{id?}")]
        public IActionResult Profile(string id)
        {
            TempData["Role"] = HttpContext.Session.GetString("Role");

            if (HttpContext.Session.GetString("Id").Equals(id))
            {
                HttpContext.Session.SetString("Its_me", "true");
                TempData["Its_me"] = "true";
            }
            else
            {
                HttpContext.Session.SetString("Its_me", "false");
                TempData["Its_me"] = "false";
            }

            var account = _dbContext.Accounts.Find(Convert.ToInt32(id));

            var roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "Viewer", Text = "Viewer" }
            };
            ViewBag.Roles = roles;

            return View("Profile", account);
        }

        [HttpPost]
        [Route("/Update/{id?}")]
        public IActionResult Update(string id, Account input)
        {
            TempData["Role"] = HttpContext.Session.GetString("Role");
            TempData["Its_me"] = HttpContext.Session.GetString("Its_me");

            var account = _dbContext.Accounts.Find(Convert.ToInt32(id));

            var roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "Viewer", Text = "Viewer" }
            };
            ViewBag.Roles = roles;

            string newPassword = Request.Form["NewPassword"];
            string rePassword = Request.Form["RePassword"];

            if (HttpContext.Session.GetString("Its_me").Equals("true"))
            {
                if (account.Password != input.Password)
                {
                    ModelState.AddModelError("Password", "Mật khẩu không chính xác!");
                    return View("Profile", account);
                }
                if (newPassword.Length < 6)
                {
                    ViewBag.ErrorPassword = "Mật khẩu quá ngắn!";
                    return View("Profile", account);
                }
                if (account.Password.Equals(newPassword))
                {
                    ViewBag.ErrorPassword = "Mật khẩu mới giống mật khẩu cũ!";
                    return View("Profile", account);
                }
                if (!newPassword.Equals(rePassword))
                {
                    ViewBag.ErrorConfirm = "Mật khẩu nhập lại không trùng khớp!";
                    return View("Profile", account);
                }

                account.Password = Request.Form["NewPassword"];
            }
            
            if(HttpContext.Session.GetString("Role").Equals("SuperAdmin") && !account.Role.Equals("SuperAdmin"))
            {
                account.Status = int.Parse(Request.Form["StatusCheck"]);
                account.Role = input.Role;
            }
            _dbContext.SaveChanges();

            ModelState.Remove("Password");

            ViewBag.SuccessMessage = "Cập nhật thành công!";
            return View("Profile", account);
        }
    }
}
