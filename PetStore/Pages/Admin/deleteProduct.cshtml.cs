using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;

namespace PetStore.Pages.Admin
{
    public class deleteProductModel : PageModel
    {
        public IActionResult OnGet(int? productId)
        {
            string? roleName = HttpContext.Session.GetString("roleName");
            if (roleName == null || roleName != "Admin")
            {
                return Redirect("/Home");
            }
            List<ProductImage> proIMG = PetStoreContext.Ins.ProductImages.Where(p => p.ProductId == productId).ToList();
            if (proIMG != null)
            {
                foreach (var image in proIMG) {
                    image.Status = "deleted";
                }
                PetStoreContext.Ins.ProductImages.UpdateRange(proIMG);
                PetStoreContext.Ins.SaveChanges();
                Product pro = PetStoreContext.Ins.Products.Where(p => p.ProductId == productId).FirstOrDefault();
                if (pro != null)
                {
                    pro.Status = "deleted";
                    PetStoreContext.Ins.Products.Update(pro);
                    PetStoreContext.Ins.SaveChanges();
                }
            }
            return Redirect("/Admin/Product");
        }
    }
}
