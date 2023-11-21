using Final_NET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace Final_NET.Controllers
{
	public class LoginController : Controller
	{
		private TravelNewsDBContext _travelNewsDbContext;
		public LoginController(TravelNewsDBContext travelNewsDbContext)
		{
			_travelNewsDbContext = travelNewsDbContext;
		}

		[HttpGet]

        public IActionResult Login()
		{
			if (HttpContext.Session.GetString("Username") != null)
				HttpContext.Session.Remove("Username");
			return View();
		}

		[HttpPost]
		public IActionResult RequestLogin(Account inputAccount)
		{
			var account = _travelNewsDbContext.Accounts.FirstOrDefault(a => a.Username == inputAccount.Username);

			ModelState.Remove("Username");
			ModelState.Remove("Password");

			if (account == null)
			{
				ModelState.AddModelError("Username", "Tên tài khoản không tồn tại!");
				return View("Login", inputAccount);
			}
			if (inputAccount.Password == null)
			{
                ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu để đăng nhập!");
                return View("Login", inputAccount);
            }
			if (!account.Password.Equals(inputAccount.Password))
			{
				ModelState.AddModelError("Password", "Mật khẩu không chính xác!");
				return View("Login", inputAccount);
			}
			if(account.Status == 0)
			{
                ModelState.AddModelError("Password", "Tài khoản của bạn đã bị khóa!");
                return View("Login", inputAccount);
            }

            HttpContext.Session.SetString("Id", account.Id.ToString());
            HttpContext.Session.SetString("Username", account.Username);
            HttpContext.Session.SetString("Role", account.Role);

			if (account.Role.Equals("Admin") || account.Role.Equals("SuperAdmin"))
				return RedirectToAction("Admin", "Admin");
			else
			{
                return RedirectToAction("Home", "Home");
            }
		}
	}
}
