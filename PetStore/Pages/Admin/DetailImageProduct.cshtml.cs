using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;

namespace PetStore.Pages.Admin
{
    public class DetailImageProductModel : PageModel
    {
        public List<ProductImage> lsProductIMG { get; set; } = new List<ProductImage>();
        string pathSave = "/tpl/img/";
        public IActionResult OnGet(int? id = 1)
        {
            string? roleName = HttpContext.Session.GetString("roleName");
            if (roleName == null || roleName != "Admin")
            {
                return Redirect("/Home");
            }
            lsProductIMG = PetStoreContext.Ins.ProductImages.Include(p => p.Product).Where(p => p.ProductId == id).ToList();
            return Page();
        }
        public async Task<IActionResult> OnPost(int? productId, IFormFile productImg, string method)
        {
            string? roleName = HttpContext.Session.GetString("roleName");
            if (roleName == null || roleName != "Admin")
            {
                return Redirect("/Home");
            }
            if (method == "update")
            {
                var ImgID = Request.Form["productIMGId"];
                ProductImage pro = PetStoreContext.Ins.ProductImages.Where(p => p.ImgId == int.Parse(ImgID)).FirstOrDefault();
                if (pro != null) {
                    var filePath = Path.Combine("wwwroot/tpl/img", productImg.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await productImg.CopyToAsync(stream);
                    }
                    string pathImg = pathSave + productImg.FileName;
                    pro.ImgUrl = pathImg;
                    pro.UpdateAt = DateTime.Now;
                    PetStoreContext.Ins.ProductImages.Update(pro);
                    await PetStoreContext.Ins.SaveChangesAsync();
                }
            }
            else if(method == "create")
            {
                List<ProductImage> productIMG = PetStoreContext.Ins.ProductImages.Where(p => p.ProductId == productId).ToList();
                if (productIMG.Count < 4)
                {
                    ProductImage pro = new ProductImage();
                    var filePath = Path.Combine("wwwroot/tpl/img", productImg.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await productImg.CopyToAsync(stream);
                    }
                    string pathImg = pathSave + productImg.FileName;
                    pro.ImgUrl = pathImg;
                    pro.CreateAt = DateTime.Now;
                    pro.ProductId = productId;
                    pro.Status = "Active";
                    PetStoreContext.Ins.ProductImages.Add(pro);
                    await PetStoreContext.Ins.SaveChangesAsync();
                }
            }

            return Redirect("/Admin/DetailImageProduct?id=" + productId);
        }
    }
}
