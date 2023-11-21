using Final_NET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace Final_NET.Controllers
{
	public class RegisterController : Controller
	{
		private TravelNewsDBContext _travelNewsDBContext;
		public RegisterController(TravelNewsDBContext travelNewsDBContext)
		{
			_travelNewsDBContext = travelNewsDBContext;
		}

		[HttpGet]
        [Route("/Register")]
        public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public IActionResult checkValidate(Account inputAccount)
		{
            if (_travelNewsDBContext.Accounts.Any(a => a.Username == inputAccount.Username))
            {
                ModelState.AddModelError("Username", "Tên tài khoản đã tồn tại.");
                return View("Register", inputAccount);
            }
            else if (_travelNewsDBContext.Accounts.Any(a => a.Email == inputAccount.Email))
            {
                ModelState.AddModelError("Email", "Tài khoản đã được sử dụng.");
                return View("Register", inputAccount);
            }
			else if(inputAccount.Password.Length < 6)
			{
                ModelState.AddModelError("Password", "Mật khẩu quá ngắn.");
                return View("Register", inputAccount);
            }
            if (inputAccount.Password != Request.Form["RePassword"])
			{
				ViewBag.ErrorMessage = "Mật khẩu nhập lại không trùng khớp!";
				return View("Register");
			}

			// Store to tempdata
			TempData["name"] = inputAccount.Username;

			HttpContext.Session.SetString("Username", inputAccount.Username);
			HttpContext.Session.SetString("Password", inputAccount.Password);
			HttpContext.Session.SetString("Email", inputAccount.Email);

            return RedirectToAction("Verification", "Verification");
		}
	}
}
