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
        public List<OrderViewModel> ordersview { get; set; } = new List<OrderViewModel>();
        public int PAGE_SIZE =10;
        public int TotalPages = 0;
        public int PageNumber =0;
        public string Status { get; set; }
        public string Payment { get; set; }
        public string s { get; set; }
        public IActionResult OnGet(string status, string payment, int? pagenum = 1)
        {
            //string? roleName = HttpContext.Session.GetString("roleName");
            //if (roleName == null || roleName == "Customer")
            //{
            //    return Redirect("/login");
            //}
            Console.WriteLine($"Processing page: {pagenum}");
            GetData(status, payment, pagenum);
            return Page();
        }
        private void GetData(string status, string payment, int? pagenum = 1)
        {
            Console.WriteLine($"Processing page: {pagenum}");
            var query = from o in _context.Orders
                        join i in _context.Infors on o.AccountId equals i.AccountId
                        join a in _context.Addresses on o.AddressId equals a.AddressId
                        join p in _context.PaymentMethods on o.PaymentMethodId equals p.MethodId
                        join s in _context.StatusOrders on o.StatusId equals s.StatusId
                        join sale in _context.Infors on o.SaleId equals sale.AccountId into saleJoin
                        from sale in saleJoin.DefaultIfEmpty()
                        select new OrderViewModel
                        {
                            OrderId = o.OrderId,
                            Image = i.Image,
                            Name = i.Fullname,
                            Status = s.StatusName,
                            CreateAt = o.CreateAt,
                            UpdateAt = o.UpdateAt,
                            Note = o.Note,
                            NoteBySale = o.NoteBySale,
                            ImageSale = sale != null ? sale.Image : null,
                            NameSale = sale != null ? sale.Fullname : "Không có sale",
                            Payment = p.MethodName,
                            Addresses = a.Address1,
                            StatusId = o.StatusId,
                            PaymentId = o.PaymentMethodId,
                        };

            if (!string.IsNullOrEmpty(status) && status != "All" && int.TryParse(status, out int statusId))
            {
                query = query.Where(x => x.StatusId == statusId);
            }
            if (!string.IsNullOrEmpty(payment) && payment != "All" && int.TryParse(payment, out int paymentId))
            {
                query = query.Where(x => x.PaymentId == paymentId);
            }

            Status = status;
            Payment = payment;

            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PAGE_SIZE);
            PageNumber = pagenum.Value;
            ordersview = query
                .Skip((pagenum.Value - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .ToList();
        }

       

        public IActionResult OnPostUpdateStatus(string orderId, string status, string sta, string payment, int? pagenum = 1)
        {   
            GetData(sta,payment, pagenum);
            Order o = _context.Orders.Where(x => x.OrderId == int.Parse(orderId)).FirstOrDefault();
            o.StatusId = int.Parse(status);
            _context.Orders.Update(o);
            _context.SaveChanges();
            return RedirectToPage();
        }

        public class OrderViewModel
        {
            public int OrderId { get; set; }
            public string Image { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public DateTime? CreateAt { get; set; }
            public DateTime? UpdateAt { get; set; }
            public string Note { get; set; }
            public string NoteBySale { get; set; }
            public string ImageSale { get; set; }
            public string NameSale { get; set; }
            public string Payment { get; set; }
            public string Addresses { get; set; }
            public int? StatusId { get; set; }
            public int? PaymentId { get; set; }
        }

    }
}
