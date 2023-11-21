using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mail;
using System.Net;
using Final_NET.Models;

namespace Final_NET.Controllers
{
    public class ForgotController : Controller
    {
        private TravelNewsDBContext _dbContext;
        public ForgotController(TravelNewsDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("/Forgot")]
        public IActionResult Forgot()
        {
            return View();
        }

        [HttpPost]
        [Route("/SendPassword")]
        public IActionResult SendPassword(Account inAccount)
        {
            var account = _dbContext.Accounts.FirstOrDefault(a => a.Username == inAccount.Username);

            if(account != null)
            {
                sendPasswordToEmail(account.Email);
                account.Password = HttpContext.Session.GetString("Password");
                _dbContext.SaveChanges();

                ViewBag.SuccessMessage = "Mật khẩu mới đã được gửi về email của bạn!";

                return View("Forgot", inAccount);
            }
            else
            {
                ViewBag.ErrorMessage = "Tên đăng nhập không tồn tại";
                return View("Forgot", inAccount);
            }
        }
        private void RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            HttpContext.Session.SetString("Password", new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray()));
        }
        private void sendPasswordToEmail(string email)
        {
            string To = email;
            string From = "nguyntrungkin091@gmail.com";

            string GoogleAppPassword = "izez egcc gpcl manx";

            RandomString(10);

            string Subject = "Mã xác thực tài khoản";
            string Body = "Bạn đã gửi yêu cầu mật khẩu mới. Mật khẩu mới của bạn là: " + HttpContext.Session.GetString("Password");

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
        }
    }
}
