using Final_NET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Xml.Linq;


namespace Final_NET.Controllers
{
	public class VerificationController : Controller
	{
		private TravelNewsDBContext _travelNewsDBContext;
		public VerificationController(TravelNewsDBContext travelNewsDBContext)
		{
			_travelNewsDBContext = travelNewsDBContext;
		}

		[HttpGet]
		[Route("/Verification")]
		public IActionResult Verification()
		{
			string email = HttpContext.Session.GetString("Email");

            TempData["name"] = email;

			sendOtpCodeToEmail(email);

			return View();
		}

		[HttpPost]
		public IActionResult CheckValidateOtp()
		{
			if(IsOtpValid())
			{
				var newAccount = new Account
				{
					Username = HttpContext.Session.GetString("Username"),
					Password = HttpContext.Session.GetString("Password"),
					Email = HttpContext.Session.GetString("Email")
				};

				_travelNewsDBContext.Accounts.Add(newAccount);
				_travelNewsDBContext.SaveChanges();
				return RedirectToAction("Home", "Home");
			}
			else
			{
				TempData["name"] = HttpContext.Session.GetString("Email");
                ViewBag.ErrorMessage = "Mã không hợp lệ hoặc đã hết hạn!";
                return View("Verification");
            }
		}

		private void sendOtpCodeToEmail(string email)
		{
			string To = email;
			string From = "nguyntrungkin091@gmail.com";

			string GoogleAppPassword = "izez egcc gpcl manx";

			Random random = new Random();
			int otp = random.Next(100000, 1000000);

			string Subject = "Mã xác thực tài khoản";
			string Body = "Đây là mã xác thực tài khoản của bản. Vui lòng không chia sẻ nó cho bất kì ai. Mã otp của bạn là:" + otp.ToString();

			var smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential(From, GoogleAppPassword),
				EnableSsl = true,
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress(From, "Travel News Page"),
				Subject = Subject,
				Body = Body,
				IsBodyHtml = true,
			};
			mailMessage.To.Add(To);

			smtpClient.Send(mailMessage);

			HttpContext.Session.SetString("OtpCode", otp.ToString());
		}

		private bool IsOtpValid()
		{
			// check validate otp code
			string storedOtpCode = HttpContext.Session.GetString("OtpCode");
			string enteredOtpCode = Request.Form["ConfirmCode"];
			bool validateOtp = string.Equals(storedOtpCode, enteredOtpCode, StringComparison.OrdinalIgnoreCase);

			// check time of otp
			bool stillTime = Convert.ToInt32(Request.Form["CountdownValue"]) > 0;

			if (validateOtp && stillTime) return true;
			return false;
		}
	}
}
