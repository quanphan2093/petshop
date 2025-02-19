using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;

namespace PetStore.Pages.Admin
{
    public class ShopKeyPartnerModel : PageModel
    {
        public List<Shop> lsShop { get; set; } = new List<Shop>();
        public int totalPage = 0;
        private int pageSize = 10;
        public int currentPage = 0;
        public IActionResult OnGet(int? current = 1)
        {
            string? roleName = HttpContext.Session.GetString("roleName");
            if (roleName == null || roleName != "Admin")
            {
                return Redirect("/Home");
            }
            var Shop = PetStoreContext.Ins.Shops.OrderBy(s => s.Status).AsQueryable();
            totalPage = Shop.Count() / pageSize;
            if (Shop.Count() % pageSize != 0) totalPage += 1;

            Shop = Shop.Skip((current.Value - 1) * pageSize).Take(pageSize);
            currentPage = current.Value;

            lsShop = Shop.ToList();
            return Page();
        }
        public async Task<IActionResult> OnPost(string method, string shopName, string address, string phone, string status, string website, string facebookUrl)
        {
            string? roleName = HttpContext.Session.GetString("roleName");
            if (roleName == null || roleName != "Admin")
            {
                return Redirect("/Home");
            }
            if (method == "create")
            {
                Shop shop = new Shop();
                shop.ShopName = shopName;
                shop.Address = address;
                shop.Phone = phone;
                shop.Status = "Active";
                shop.Website = website;
                shop.FacebookUrl = facebookUrl;
                shop.CreateAt = DateTime.Now;
                try
                {
                    PetStoreContext.Ins.Shops.Add(shop);
                    await PetStoreContext.Ins.SaveChangesAsync();
                }catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else if (method == "update")
            {
                int shopId = int.Parse(Request.Form["shopId"]);
                try
                {
                    Shop shop = PetStoreContext.Ins.Shops.Where(s => s.ShopId == shopId).FirstOrDefault();
                    if (shop != null)
                    {
                        shop.ShopName = shopName;
                        shop.Address = address;
                        shop.ShopName = shopName;
                        shop.Address = address;
                        shop.Phone = phone;
                        shop.Status = status;
                        shop.Website = website;
                        shop.FacebookUrl = facebookUrl;
                        PetStoreContext.Ins.Shops.Update(shop);
                        await PetStoreContext.Ins.SaveChangesAsync();

                        if(status == "Inactive")
                        {
                            List<Product> product = PetStoreContext.Ins.Products.Where(p => p.ShopId == shopId).ToList();
                            foreach (Product productItem in product) {
                                productItem.Status = "deleted";
                            }
                            PetStoreContext.Ins.Products.UpdateRange(product);
                            await PetStoreContext.Ins.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex) {
                    throw new Exception($"{ex.Message}");
                }
                
            }
            return Redirect("/Admin/Shop");
        }
    }
}
