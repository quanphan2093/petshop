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
        public void OnGet(int? id)
        {
            if (id == null)
            {
                return;
            }

            // Query the product from the database
            product = PetStoreContext.Ins.Products
                                      .Include(p => p.ProductImages)
                                      .FirstOrDefault(p => p.ProductId == id);
            var listfeedbacks = PetStoreContext.Ins.Feedbacks.Include(a => a.Account).ThenInclude(i => i.Infors)
                .Where(f => f.ProductId == id).AsQueryable();

            lsfeedbacks = listfeedbacks.ToList();

            var listImage = PetStoreContext.Ins.ProductImages.Where(p => p.ProductId == id).AsQueryable();
            lsImage = listImage.ToList();

            var listProduct = PetStoreContext.Ins.Products.Include(p => p.Category).OrderBy(p => p.UnitOrdered).Take(5).AsQueryable();
            lsProduct = listProduct.ToList();

        }
    }
}
