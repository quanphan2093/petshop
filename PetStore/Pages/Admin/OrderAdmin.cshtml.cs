using ClosedXML.Excel;
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
            string? roleName = HttpContext.Session.GetString("roleName");
            if (roleName == null || roleName == "Customer")
            {
                return Redirect("/login");
            }
            GetData(status, payment, pagenum);
            return Page();
        }
        private void GetData(string status, string payment, int? pagenum = 1)
        {
            paymentMethods = _context.PaymentMethods.ToList();
            statusOrders = _context.StatusOrders.ToList();
            var query = from o in _context.Orders
                        join i in _context.Infors on o.AccountId equals i.AccountId
                        join a in _context.Addresses on o.AddressId equals a.AddressId
                        join p in _context.PaymentMethods on o.PaymentMethodId equals p.MethodId
                        join s in _context.StatusOrders on o.StatusId equals s.StatusId
                        join sale in _context.Infors on o.SaleId equals sale.AccountId into saleJoin
                        from sale in saleJoin.DefaultIfEmpty()
                        orderby o.CreateAt ascending
                        select new OrderViewModel
                        {
                            OrderId = o.OrderId,
                            Image = i.Image,
                            Name = a.FullNameCustomer,
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

        public IActionResult OnGetExport()
        {
            PetStoreContext context = new PetStoreContext();
            var listItem = context.OrderDetails
                                    .Include(x => x.Order)
                                        .ThenInclude(x => x.Address)
                                    .Include(x => x.Order)
                                        .ThenInclude(x => x.Status)
                                    .Include(x => x.Product)
                                        .ThenInclude(x => x.Shop)
                                    .OrderBy(x => x.Order.CreateAt)
                                    .ToList();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Danh sách sản phẩm");
                worksheet.Cell(1, 1).Value = "Tên sản phẩm";
                worksheet.Cell(1, 2).Value = "Tên Shop";
                worksheet.Cell(1, 3).Value = "Số lượng";
                worksheet.Cell(1, 4).Value = "Giá";
                worksheet.Cell(1, 5).Value = "Ngày đặt";
                worksheet.Cell(1, 6).Value = "Người nhận";
                worksheet.Cell(1, 7).Value = "Số điện thoại người nhận";
                worksheet.Cell(1, 8).Value = "Địa chỉ";
                worksheet.Cell(1, 9).Value = "trạng thái đơn hàng";


                int row = 2;
                foreach(var item in listItem)
                {
                    worksheet.Cell(row, 1).Value = item.Product.ProductName;
                    worksheet.Cell(row, 2).Value = item.Product.Shop.ShopName;
                    worksheet.Cell(row, 3).Value = item.Quantity;
                    worksheet.Cell(row, 4).Value = item.Total;
                    worksheet.Cell(row, 5).Value = item.Order.CreateAt;
                    worksheet.Cell(row, 6).Value = item.Order.Address.FullNameCustomer;
                    worksheet.Cell(row, 7).Value = item.Order.Address.Phone;
                    worksheet.Cell(row, 8).Value = item.Order.Address.Address1;
                    worksheet.Cell(row, 9).Value = item.Order.Status.StatusName;
                    row++;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportData.xlsx");
                }

            }
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
