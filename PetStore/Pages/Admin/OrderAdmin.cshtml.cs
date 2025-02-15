using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;
using System.IO.Pipes;

namespace PetStore.Pages.Admin
{
    public class OrderAdminModel : PageModel
    {
        private readonly PetStoreContext _context;
        public OrderAdminModel(PetStoreContext context)
        {
            _context = context;
        }
        public List<Order> orders { get; set; } = new List<Order>();
        public List<Account> accounts { get; set; } = new List<Account>();
        public List<Infor> infors { get; set; } = new List<Infor>();
        public List<PaymentMethod> paymentMethods { get; set; } = new List<PaymentMethod>();
        public List<StatusOrder> statusOrders { get; set; } = new List<StatusOrder>();
        public List<Address> addresses { get; set; } = new List<Address>();
        public const int PAGE_SIZE =10;
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public string Status { get; set; }
        public string Payment { get; set; }
        public string s { get; set; }

        private void GetData(int? page, string status, string payment)
        {
            
            int pageNumber = page ?? 1;

            accounts = _context.Accounts.ToList();
            infors = _context.Infors.ToList();
            orders = _context.Orders.ToList();
            addresses = _context.Addresses.ToList();
            statusOrders = _context.StatusOrders.ToList();
            paymentMethods = _context.PaymentMethods.ToList();

            var order = from o in orders
                        join i in infors on o.AccountId equals i.AccountId
                        join a in addresses on o.AddressId equals a.AddressId
                        join p in paymentMethods on o.PaymentMethodId equals p.MethodId
                        join s in statusOrders on o.StatusId equals s.StatusId
                        join sale in infors on o.SaleId equals sale.AccountId into saleJoin
                        from sale in saleJoin.DefaultIfEmpty()
                        select new
                        {
                            OrderId = o.OrderId,
                            image = i.Image,
                            name = i.Fullname,
                            status = s.StatusName,
                            createAt = o.CreateAt,
                            updateAt = o.UpdateAt,
                            note = o.Note,
                            notebySale = o.NoteBySale,
                            imageSale = sale != null ? sale.Image : null,
                            nameSale = sale != null ? sale.Fullname : "Không có sale",
                            payment = p.MethodName,
                            addresses = a.Address1,
                            statusId = o.StatusId,
                            paymentId = o.PaymentMethodId,
                        };
            if (!string.IsNullOrEmpty(status) && status != "All" && int.TryParse(status, out int statusId))
            {
                order = order.Where(x => x.statusId == statusId);
            }
            if (!string.IsNullOrEmpty(payment) && payment != "All" && int.TryParse(payment, out int paymentId))
            {
                order = order.Where(x => x.paymentId == paymentId);
            }
            Status = status;
            Payment = payment;
            var totalItems = order.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PAGE_SIZE);
            TotalPages = totalPages;
            PageNumber = pageNumber;
            var paginatedOrders = order
                .Skip((pageNumber - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .ToList();
            ViewData["order"] = paginatedOrders;
        }
        public IActionResult OnGet(int? page, string status, string payment)
        {
            string? roleName = HttpContext.Session.GetString("roleName");
            if (roleName == null || roleName == "Customer")
            {
                return Redirect("/login");
            }
            GetData(page, status, payment);
            return Page();
        }

        public IActionResult OnPostUpdateStatus(string orderId, string status, int? page, string sta, string payment)
        {   
            GetData(page,sta,payment);
            Order o = _context.Orders.Where(x => x.OrderId == int.Parse(orderId)).FirstOrDefault();
            o.StatusId = int.Parse(status);
            _context.Orders.Update(o);
            _context.SaveChanges();
            return RedirectToPage();
        }
    }
}
