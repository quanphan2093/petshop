using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;
using PetStore.Pages.Common;

namespace PetStore.Pages.Customer
{
    public class HomeModel : PageModel
    {   
        public List<Product> products { get; set; }= new List<Product>();
        public void OnGet()
        {
            products= PetStoreContext.Ins.Products.ToList();
        }

		private static void SendMail(string email, string token, string pass)
		{
			string url = $"https://localhost:7144/active?token={token}";
			string body = $"Hello {email},\n\nYour activation {url}. Please enter this code in the application to activate your account \n\n Your password is: {pass}";
			IConfiguration config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", true, true)
				.Build();
			string emailFrom = config["InforMail:email"];
			string passFrom = config["InforMail:password"];
			Mail.instance.sendMail(emailFrom, passFrom, email, body, "Active Account");
		}

		public IActionResult OnPost()
		{
			try
			{
                string email = Request.Form["email"];
                string pass = GenerateRandomString(6);
                if (string.IsNullOrEmpty(email))
                {
                    TempData["error"] = "Email is not null";
                    return Redirect("/Home");
                }
                var acc = PetStoreContext.Ins.Accounts.Where(x => x.Email.Equals(email)).FirstOrDefault();
                if (acc != null)
                {
                    TempData["error"] = "Email has been registered";
                    return Redirect("/Home");
                }
                string hash = BCrypt.Net.BCrypt.HashPassword(pass);
                var account = new Account();
                account.Email = email;
                account.Password = hash;
                account.CreateAt = DateTime.Now;
                account.RoleId = 2;
                PetStoreContext.Ins.Accounts.Add(account);
                PetStoreContext.Ins.SaveChanges();
                string token = Guid.NewGuid().ToString();
                SendMail(email, token, pass);
                return Redirect("/Login");
            }
            catch(Exception ex)
			{
                Console.WriteLine(ex.Message);
                TempData["error"] = "An unexpected error occurred.";
                return Redirect("/Home");
            }
		}

		private string GenerateRandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var random = new Random();
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}
