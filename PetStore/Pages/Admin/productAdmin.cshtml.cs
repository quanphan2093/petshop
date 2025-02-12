using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;

namespace PetStore.Pages.Admin
{
    public class productAdminModel : PageModel
    {
        public List<Product> lsProduct { get; set; } = new List<Product>();
        public int totalPage = 0;
        private int pageSize = 10;
        public int currentPage = 0;
        string pathSave = "/tpl/img/";
        public List<Category> lsCategory { get; set; } = new List<Category>();
        public void OnGet(int? current = 1)
        {

            LoadData(current);
        }

        public void LoadData(int? current = 1)
        {
            var products = PetStoreContext.Ins.Products.Include(p => p.Category).Include(pi => pi.ProductImages).AsQueryable();

            totalPage = products.Count() / pageSize;
            if (products.Count() % pageSize != 0) totalPage += 1;

            products = products.Skip((current.Value - 1) * pageSize).Take(pageSize);
            currentPage = current.Value;

            lsProduct = products.ToList();
            lsCategory = PetStoreContext.Ins.Categories.ToList();

        }

        public async Task OnPost(string method, IFormFile img, IFormFile productImg) {

            if (method == "create") { 
                string name = Request.Form["name"];
                string detail = Request.Form["detail"].ToString();
                var filePath = Path.Combine("wwwroot/tpl/img", img.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }
                string pathImg = pathSave + img.FileName;
                int price = int.Parse(Request.Form["price"]);
                double? discount;
                string discountValue = Request.Form["discount"];
                discount = double.TryParse(discountValue, out double resultDiscount) ? resultDiscount : 0;
                string status = Request.Form["status"];
                int categoryId = int.Parse(Request.Form["category"]);
                int? size;
                string sizeValue = Request.Form["size"];
                size = int.TryParse(discountValue, out int resultSize) ? resultSize : null;
                int unitInStock = int.Parse(Request.Form["unitInStock"]);

                Product product = new Product();
                product.ProductName = name;
                product.Details = System.Text.Json.JsonSerializer.Serialize(detail).Trim('"');
                product.CategoryId = categoryId;
                product.Size = size;
                product.UnitInStock = unitInStock;
                product.Image = pathImg;
                product.Price = price;
                product.Discount = discount;
                product.Status = "Available";
                product.UnitOrdered = 0;
                product.CreateAt = DateTime.Now;
                try
                {
                    PetStoreContext.Ins.Products.Add(product);
                    await PetStoreContext.Ins.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                ProductImage proImg = new ProductImage();
                proImg.ImgUrl = pathImg;
                proImg.CreateAt = DateTime.Now;
                proImg.Status = "Active";
                proImg.ProductId = product.ProductId;
                try
                {
                    PetStoreContext.Ins.ProductImages.Add(proImg);
                    await PetStoreContext.Ins.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            else if (method == "update") Console.WriteLine("update");

            LoadData();
        }

    }
}
