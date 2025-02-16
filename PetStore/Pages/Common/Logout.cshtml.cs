using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;

namespace PetStore.Pages.Common
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Remove("role");
            HttpContext.Session.Remove("roleName");
            HttpContext.Session.Remove("acc");
            HttpContext.Session.Remove("email");
            return Redirect("/Home");
        }
    }
}
