using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;

namespace PetStore.Pages.Admin
{
    public class DeleteImgProductModel : PageModel
    {
        public IActionResult OnGet(int? id = 1)
        {
            string? roleName = HttpContext.Session.GetString("roleName");
            if (roleName == null || roleName != "Admin")
            {
                return Redirect("/Home");
            }
            ProductImage proImg = PetStoreContext.Ins.ProductImages.Where(p => p.ImgId == id).FirstOrDefault();
            int? productId = 1;
            if (proImg != null)
            {
                productId = proImg.ProductId;
                PetStoreContext.Ins.ProductImages.Remove(proImg);
                PetStoreContext.Ins.SaveChanges();
            }
            return Redirect("/Admin/DetailImageProduct?id=" + productId);
        }
    }
}
