using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;

namespace PetStore.Pages.Customer
{
    public class ProductDetailsModel : PageModel
    {
        [BindProperty]
        public Product product {  get; set; } = new Product();
        [BindProperty]
        public List<ProductImage> lsImage { get; set; } = new List<ProductImage>();
        [BindProperty]
        public List<Feedback> lsfeedbacks { get; set; } = new List<Feedback>();

        [BindProperty]
        public List<Product> lsProduct { get; set; } = new List<Product>();
        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return Redirect("/Home");
            }

            product = PetStoreContext.Ins.Products
                                      .Include(p => p.ProductImages).Where(p => p.Status == "Available")
                                      .FirstOrDefault(p => p.ProductId == id);
            if (product == null) return Redirect("/Home");
            var listfeedbacks = PetStoreContext.Ins.Feedbacks.Include(a => a.Account).ThenInclude(i => i.Infors)
                .Where(f => f.ProductId == id).AsQueryable();
            if(listfeedbacks == null) return Redirect("/Home");
            lsfeedbacks = listfeedbacks.ToList();

            var listImage = PetStoreContext.Ins.ProductImages.Where(p => p.ProductId == id).AsQueryable();
            if(listImage == null) return Redirect("/Home");
            lsImage = listImage.ToList();

            var listProduct = PetStoreContext.Ins.Products.Include(p => p.Category).OrderBy(p => p.UnitOrdered).Take(5).AsQueryable();
            if (listProduct == null) return Redirect("/Home");
            lsProduct = listProduct.ToList();
            return Page();

        }
    }
}
