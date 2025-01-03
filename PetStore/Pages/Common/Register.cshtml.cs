﻿using Microsoft.AspNetCore.Mvc;
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
                ViewData["messageErrorRegister"] = "The email field is not empty.";
                return Page();
            }
            if (!Regex.IsMatch(email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$"))
            {
                ViewData["messageErrorRegister"] = "Please input a valid email";
                return Page();
            }
            if (string.IsNullOrEmpty(pass))
            {
                ViewData["messageErrorRegister"] = "Password cannot be null";
                return Page();
            }
            var account = PetStoreContext.Ins.Accounts.Where(x => x.Email.Equals(email)).FirstOrDefault();
            if (account != null)
            {
                ViewData["messageErrorRegister"] = "Email existed";
                return Page();
            }
            if (!pass.Trim().Equals(comfirm.Trim()))
            {
                ViewData["messageErrorRegister"] = "Confirm password does not match.";
                return Page();
            }
            if (!Regex.IsMatch(pass, "(?=.*[A-Z])"))
            {
                ViewData["messageErrorRegister"] = "The password must contain at least 1 uppercase letter.";
                return Page();
            }

            if (!Regex.IsMatch(pass, "(?=.*[a-z])"))
            {
                ViewData["messageErrorRegister"] = "The password must contain at least 1 lowercase letter.";
                return Page();
            }

            if (!Regex.IsMatch(pass, "(?=.*[\\W_])"))
            {
                ViewData["messageErrorRegister"] = "The password must contain at least 1 special character.";
                return Page();
            }

            if (pass.Length < 8)
            {
                ViewData["messageErrorRegister"] = "The password must be at least 8 characters long.";
                return Page();
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(pass);
            var acc = new Account();
            acc.Email = email;
            acc.Password = hash;
            acc.CreateAt = DateTime.Now;
            acc.RoleId = 2;
            PetStoreContext.Ins.Accounts.Add(acc);
            PetStoreContext.Ins.SaveChanges();
            string token = Guid.NewGuid().ToString();
            SendMail(email, token, pass);
            TempData["successRegister"] = "Register successfull. Please check email to active account";
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
