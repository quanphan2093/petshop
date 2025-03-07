using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;
using PetStore.Pages.Common;
using System.Security.Principal;
using System.Text.Json;

namespace PetStore.Pages.Admin
{
    public class productAdminModel : PageModel
    {
        private readonly AzureBlobService _blobService;
        public productAdminModel(AzureBlobService blobService)
        {
            _blobService = blobService;
        }
        public List<Product> lsProduct { get; set; } = new List<Product>();
        public int totalPage = 0;
        private int pageSize = 10;
        public int currentPage = 0;
        string pathSave = "/tpl/img/";
        public List<Category> lsCategory { get; set; } = new List<Category>();
        public List<Shop> lsShop { get; set; } = new List<Shop>();
        public IActionResult OnGet(int? current = 1)
        {
            string? roleName = HttpContext.Session.GetString("roleName");
            if (roleName == null || roleName != "Admin")
            {
                return Redirect("/Home");
            }
            LoadData(current);
            return Page();
        }
        public void LoadData(int? current = 1)
        {
            var products = PetStoreContext.Ins.Products.Include(p => p.Category).Include(pi => pi.ProductImages).Include(s => s.Shop).Where(p => p.Status != "deleted").OrderByDescending(p => p.CreateAt).AsQueryable();

            totalPage = products.Count() / pageSize;
            if (products.Count() % pageSize != 0) totalPage += 1;

            products = products.Skip((current.Value - 1) * pageSize).Take(pageSize);
            currentPage = current.Value;

            lsProduct = products.ToList();
            lsCategory = PetStoreContext.Ins.Categories.ToList();
            lsShop = PetStoreContext.Ins.Shops.Where(s => s.Status == "Active").ToList();
        }

        public async Task<IActionResult> OnPost(IFormFile img, IFormFile productImg, string? method = "null")
        {
            string? roleName = HttpContext.Session.GetString("roleName");
            if (roleName == null || roleName != "Admin")
            {
                return Redirect("/Home");
            }
            if (method == "create")
            {
                string name = Request.Form["name"];
                string detail = Request.Form["detail"].ToString();
                string fileName = Path.GetFileName(img.FileName);
                string pathImg = "";
                using (var stream = img.OpenReadStream())
                {
                    pathImg = await _blobService.UploadImageAsync(stream, fileName);
                }
                decimal price = decimal.Parse(Request.Form["price"]);
                double? discount;
                string discountValue = Request.Form["discount"];
                discount = double.TryParse(discountValue, out double resultDiscount) ? resultDiscount : 0;
                string status = Request.Form["status"];
                int categoryId = int.Parse(Request.Form["category"]);
                int? size;
                string sizeValue = Request.Form["size"];
                size = int.TryParse(discountValue, out int resultSize) ? resultSize : null;
                int unitInStock = int.Parse(Request.Form["unitInStock"]);
                int shopId = int.Parse(Request.Form["shop"]);
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
                product.ShopId = shopId;
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
            else if (method == "update")
            {
                int productId = int.Parse(Request.Form["productId"]);
                Product pro = PetStoreContext.Ins.Products.Where(p => p.ProductId == productId).FirstOrDefault();
                if (pro != null)
                {
                    pro.ProductName = Request.Form["productName"];
                    string detail = Request.Form["productDetail"];
                    pro.Details = detail.Replace("\r\n", "\\r\\n").Replace("\n", "\\n");
                    string pathImg = pro.Image;
                    if (productImg != null)
                    {
                        string fileName = Path.GetFileName(productImg.FileName);
                        using (var stream = productImg.OpenReadStream())
                        {
                            pathImg = await _blobService.UploadImageAsync(stream, fileName);
                        }
                    }
                    pro.Image = pathImg;
                    pro.Price = decimal.Parse(Request.Form["proprice"]);
                    double? discount;
                    string discountValue = Request.Form["prodiscount"];
                    discount = double.TryParse(discountValue, out double resultDiscount) ? resultDiscount : 0;
                    pro.Discount = discount;
                    pro.Status = Request.Form["prostatus"];
                    pro.CategoryId = int.Parse(Request.Form["productCate"]);
                    int? size;
                    string sizeValue = Request.Form["prosize"];
                    size = int.TryParse(discountValue, out int resultSize) ? resultSize : null;
                    pro.Size = size;
                    pro.UnitInStock = int.Parse(Request.Form["prounitInStock"]);
                    pro.UpdateAt = DateTime.Now;
                    int shopId = int.Parse(Request.Form["shop"]);
                    pro.ShopId = shopId;

                    PetStoreContext.Ins.Products.Update(pro);
                    await PetStoreContext.Ins.SaveChangesAsync();

                    ProductImage proImg = PetStoreContext.Ins.ProductImages.Where(p => p.ProductId == pro.ProductId).FirstOrDefault();
                    if (proImg != null)
                    {
                        proImg.ImgUrl = pro.Image;
                        proImg.UpdateAt = DateTime.Now;
                        PetStoreContext.Ins.ProductImages.Update(proImg);
                        await PetStoreContext.Ins.SaveChangesAsync();
                    }
                }
            }

            LoadData();
            return Redirect("/Admin/Product");
        }

    }
}
