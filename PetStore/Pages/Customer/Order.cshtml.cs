using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;

namespace PetStore.Pages.Customer
{
    public class OrderModel : PageModel
    {   
        public List<Order> orders = new List<Order>();
        public List<Category> categories = new List<Category>();
        public List<Product> products = new List<Product>();
        public List<OrderDetail> ordersDetail = new List<OrderDetail>();
        public List<StatusOrder> statusOrders = new List<StatusOrder>();
        public PetStoreContext _context;
        public string s;
        public string Search;
        public OrderModel(PetStoreContext context)
        {
            _context=context;
        }
        public IActionResult OnGet(string status, string search)
        {
            int? accId = HttpContext.Session.GetInt32("acc");
            if (accId == null)
            {
                return RedirectToPage("/Common/Login");
            }
            products =_context.Products.ToList();
            categories=_context.Categories.ToList();
            ordersDetail=_context.OrderDetails.ToList();
            statusOrders=_context.StatusOrders.ToList();
            orders=_context.Orders.ToList();
            var order = from o in orders
                        join od in ordersDetail on o.OrderId equals od.OrderId
                        join p in products on od.ProductId equals p.ProductId
                        join c in categories on p.CategoryId equals c.CategoryId
                        join s in statusOrders on o.StatusId equals s.StatusId
                        where o.AccountId == accId
                        select new
                        {
                            OrderId = o.OrderId,
                            ProductName = p.ProductName,
                            Image = p.Image,
                            CategoryName = c.CategoryName,
                            Status = s.StatusName,
                            Quantity = od.Quantity,
                            Price = p.Price,
                            CreateAt = o.CreateAt,
                            Total = od.Quantity * p.Price
                        };
            if (!string.IsNullOrEmpty(status))
            {
                order = order.Where(x => x.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }
            if(!string.IsNullOrEmpty(search))
            {
                order= order.Where(x => x.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            order = order.OrderByDescending(x => x.CreateAt);
            s = status;
            Search = search;
            ViewData["order"]= order.ToList();
            return Page();
        }
    }
}
