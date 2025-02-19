using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;

namespace PetStore.Pages.Customer
{
    public class ClickBannerModel : PageModel
    {
        public IActionResult OnGet(int? id = 1)
        {
            Banner banner = PetStoreContext.Ins.Banners.Where(b => b.BannerId == id).FirstOrDefault();
            if(banner != null)
            {
                banner.ClickCount += 1;
                PetStoreContext.Ins.Banners.Update(banner);
                PetStoreContext.Ins.SaveChanges();
                return Redirect(banner.BannerUrl);
            }
            return Redirect("/Home");
        }
    }
}
