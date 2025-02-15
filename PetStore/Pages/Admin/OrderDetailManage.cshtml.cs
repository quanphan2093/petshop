using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;

namespace PetStore.Pages.Admin
{
    public class OrderDetailManageModel : PageModel
    {   
        private readonly PetStoreContext _context;
        public OrderDetailManageModel(PetStoreContext context)
        {
            _context = context;
        }
        public List<OrderDetail> orderDetails { get;set; }= new List<OrderDetail>();
        public int Total { get; set; }  
        public IActionResult OnGet(int? id)
        {
            string? roleName = HttpContext.Session.GetString("roleName");
            if (roleName == null || roleName == "Customer")
            {
                return Redirect("/login");
            }
            orderDetails =_context.OrderDetails.Include(x => x.Product).Where(x => x.OrderId == id).ToList();
            int total = 0;
            foreach (var order in orderDetails)
            {
                total += (int)order.Total;
            }
            Total=total;
            return Page();
        }
    }
}
