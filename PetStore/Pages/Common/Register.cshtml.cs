using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;
using System.Data;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace PetStore.Pages.Common
{
    public class RegisterModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            string email = Request.Form["email"];
            string pass = Request.Form["pass"];
            string comfirm = Request.Form["comfirm"];
            if (string.IsNullOrEmpty(email))
            {
                ViewData["messageErrorRegister"] = "Trường email không được để trống.";
                return Page();
            }
            if (!Regex.IsMatch(email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$"))
            {
                ViewData["messageErrorRegister"] = "Vui lòng nhập một địa chỉ email hợp lệ.";
                return Page();
            }
            if (string.IsNullOrEmpty(pass))
            {
                ViewData["messageErrorRegister"] = "Mật khẩu không được để trống.";
                return Page();
            }
            Account account = PetStoreContext.Ins.Accounts.Where(x => x.Email.Equals(email)).FirstOrDefault();
            if (account != null)
            {
                ViewData["messageErrorRegister"] = "Email đã tồn tại.";
                return Page();
            }
            if (!pass.Trim().Equals(comfirm.Trim()))
            {
                ViewData["messageErrorRegister"] = "Mật khẩu xác nhận không khớp";
                return Page();
            }
            if (!Regex.IsMatch(pass, "(?=.*[A-Z])"))
            {
                ViewData["messageErrorRegister"] = "Mật khẩu phải chứa ít nhất 1 chữ cái viết hoa";
                return Page();
            }

            if (!Regex.IsMatch(pass, "(?=.*[a-z])"))
            {
                ViewData["messageErrorRegister"] = "Mật khẩu phải chứa ít nhất 1 chữ cái viết thường.";
                return Page();
            }

            if (!Regex.IsMatch(pass, "(?=.*[\\W_])"))
            {
                ViewData["messageErrorRegister"] = "Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt.";
                return Page();
            }

            if (pass.Length < 8)
            {
                ViewData["messageErrorRegister"] = "Mật khẩu phải có ít nhất 8 ký tự.";
                return Page();
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(pass);
            Account acc = new Account();
            acc.Email = email;
            acc.Password = hash;
            acc.CreateAt = DateTime.Now;
            acc.RoleId = 2;
            acc.State = "Active";
            PetStoreContext.Ins.Accounts.Add(acc);
            PetStoreContext.Ins.SaveChanges();
            //string token = Guid.NewGuid().ToString();
            //SendMail(email, token, pass);
            //TempData["successRegister"] = "Register successfull. Please check email to active account";
            return Redirect("/Login");
        }

        private static void SendMail(string email, string token, string pass)
        {
            string url = $"https://localhost:7144/active?token={token}";
            string body = $"Hello {email},\n\nYour activation {url}. Please enter this code in the application to activate your account";
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            string emailFrom = config["InforMail:email"];
            string passFrom = config["InforMail:password"];
            Mail.instance.sendMail(emailFrom, passFrom, email, body, "Active Account");
        }
    }
}
