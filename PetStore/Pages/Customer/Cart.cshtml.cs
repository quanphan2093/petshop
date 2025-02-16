using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;

namespace PetStore.Pages.Customer
{
    public class CartModel : PageModel
    {
        [BindProperty]
        public int proId { get; set; } = 0;
        [BindProperty]
        public List<ShoppingCart> lsCart { get; set; } = new List<ShoppingCart>();
        [BindProperty]
        public int totalPro { get; set; } = 0;
        public void OnGet()
        {
            int? acc = HttpContext.Session.GetInt32("acc");
            if (acc == null)
            {
                Response.Redirect("/login");
                return;
            }
            lsCart = PetStoreContext.Ins.ShoppingCarts.Include(p => p.Product)
                .ThenInclude(p => p.Category).Where(c => c.AccountId == acc).ToList();
            if(lsCart == null) Response.Redirect("/Home");
            foreach (var p in lsCart)
            {
                totalPro = totalPro + (int)p.Quantity;
            }

        }
        public JsonResult OnPost(int productId, int quantity)
        {
            int? acc = HttpContext.Session.GetInt32("acc");
            if (!acc.HasValue) {
                return new JsonResult(new { success = false, message = "Vui lòng login trước!" });
            }
            try
            {
                if (productId <= 0 || quantity <= 0)
                {
                    return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ!" });
                }

                Product product = PetStoreContext.Ins.Products.Find(productId);
                if (product == null) {
                    return new JsonResult(new { success = false, message = "Không tìm thấy sản phẩm!" });
                }

                if(product.UnitInStock < quantity || product.UnitInStock == null)
                {
                    return new JsonResult(new { success = false, message = "Không đủ sản phẩm trong kho!" });
                }

                ShoppingCart cart = PetStoreContext.Ins.ShoppingCarts.Where(c => c.AccountId == acc && c.ProductId == productId).FirstOrDefault();
                if (cart == null) {
                    cart = new ShoppingCart();
                    cart.AccountId = acc;
                    cart.Quantity = quantity;
                    cart.ProductId  = productId;
                    cart.CreateAt = DateTime.Now;
                    PetStoreContext.Ins.ShoppingCarts.Add(cart);
                    PetStoreContext.Ins.SaveChanges();
                }
                else
                {
                    cart.Quantity += quantity;
                    cart.UpdateAt = DateTime.Now;
                    PetStoreContext.Ins.ShoppingCarts.Update(cart);
                    PetStoreContext.Ins.SaveChanges();
                }
                return new JsonResult(new { success = true, message = "Thêm vào giỏ hàng thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new JsonResult(new { success = false, message = "Đã xảy ra lỗi trên server." });
            }
        }

        public JsonResult OnPostUpdateQuantity(int[] cartIds, int[] quantities)
        {
            int? acc = HttpContext.Session.GetInt32("acc");
            if (!acc.HasValue)
            {
                return new JsonResult(new { success = false, message = "Vui lòng login trước!" });
            }
            try
            {
                for (int i=0; i < cartIds.Length; i++) {
                    ShoppingCart cart = PetStoreContext.Ins.ShoppingCarts.Where(c => c.CartId == cartIds[i]).Include(p => p.Product).FirstOrDefault();
                    if(cart != null)
                    {
                        if(quantities[i] <= cart.Product.UnitInStock)
                        {
                            if(cart.Quantity != quantities[i])
                            {
                                if (quantities[i] != 0)
                                {
                                    cart.Quantity = quantities[i];
                                    cart.UpdateAt = DateTime.Now;
                                    PetStoreContext.Ins.ShoppingCarts.Update(cart);
                                    PetStoreContext.Ins.SaveChanges();
                                }
                                else if (quantities[i] == 0)
                                {
                                    PetStoreContext.Ins.ShoppingCarts.Remove(cart);
                                    PetStoreContext.Ins.SaveChanges();
                                }
                            
                            }
                        }
                        else
                        {
                            return new JsonResult(new { success = false, message = "Số lượng sản phẩm không đủ!" });
                        }
                        
                    }
                }
                return new JsonResult(new { success = true, message = "thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Đã xảy ra lỗi trên server." });
            }
        }
    }
}
