﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;
using Microsoft.EntityFrameworkCore;

namespace PetStore.Pages
{
    public class LoginModel : PageModel
    {   
        
        public void OnGet()
        {
            
        }

        public IActionResult OnPost()
        {
			string user = Request.Form["email"];
			string pass = Request.Form["pass"];
			var account = PetStoreContext.Ins.Accounts.Where(x => x.Email.Equals(user) && x.State != "Inactive").Include(a => a.Role).FirstOrDefault();
			if (account == null)
			{
				ViewData["error"] = "Sai Email hoặc tài khoản đã bị chặn!!";
				return Page();
			}
			bool password = BCrypt.Net.BCrypt.Verify(pass, account.Password);
			if (!password)
			{
				ViewData["error"] = "Mật khẩu không đúng!";
				return Page();
			}
			HttpContext.Session.SetInt32("role", account.Role.RoleId);
            HttpContext.Session.SetString("roleName", account.Role.RoleName);
            HttpContext.Session.SetInt32("acc", account.AccountId);
			HttpContext.Session.SetString("email", account.Email);
			return Redirect("/Home");
		}
    }
}
