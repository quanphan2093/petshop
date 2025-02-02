﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;
using PetStore.Pages.Common;
using System.Text.Json;
using System.IO;
using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;

namespace PetStore.Pages.Customer
{
    public class CheckoutModel : PageModel
    {
        [BindProperty]
        public List<ShoppingCart> lsCart { get; set; } = new List<ShoppingCart>();
        [BindProperty]
        public int totalPro { get; set; } = 0;
        [BindProperty]
        public List<PaymentMethod> lspayMethod { get; set; } = new List<PaymentMethod>();

        public void OnGet()
        {
            int? acc = HttpContext.Session.GetInt32("acc");
            if (acc == null)
            {
                Response.Redirect("/login");
                return;
            }
            lsCart = PetStoreContext.Ins.ShoppingCarts.Include(p => p.Product)
                .ThenInclude(p => p.Category).Include(a => a.Account).Where(c => c.AccountId == acc).ToList();
            if(lsCart == null || lsCart.Count == 0)
            {
                Response.Redirect("/Cart");
                return;
            }
            lspayMethod = PetStoreContext.Ins.PaymentMethods.Where(pm => pm.Status == "active").ToList();
            foreach(var p in lsCart)
            {
                totalPro = totalPro + (int)p.Quantity;
            }
        }
        public async Task<IActionResult> OnPostSubmitOrder(string email, string fullname, string phone, string address, string city, string district, string ward, string note, int payment)
        {
            int? acc = HttpContext.Session.GetInt32("acc");
            if (acc == null)
            {
                return Redirect("/login");
            }
            using var client = new HttpClient();
            var responseCity = await client.GetStringAsync("https://provinces.open-api.vn/api/?depth=1");
            var cities = JsonSerializer.Deserialize<List<Province>>(responseCity);
            var selectedCity = cities.FirstOrDefault(p => p.code.ToString() == city);
            string cityName = selectedCity != null ? selectedCity.name : "null";

            var responseDistrict = await client.GetStringAsync("https://provinces.open-api.vn/api/p/" + selectedCity.code + "?depth=2");
            var districtsJson = JsonSerializer.Deserialize<DistrictResponse>(responseDistrict);
            List<District> districts = districtsJson.districts.ToList();
            var selectedDistrict = districts.FirstOrDefault(p => p.code.ToString() == district);
            string districtName = selectedDistrict != null ? selectedDistrict.name : "null";

            var responseWard = await client.GetStringAsync("https://provinces.open-api.vn/api/d/" + selectedDistrict.code + "?depth=2");
            var wardJson = JsonSerializer.Deserialize<WardResponse>(responseWard);
            List<Ward> wards = wardJson.wards.ToList();
            var selectedWards = wards.FirstOrDefault(p => p.code.ToString() == ward);
            string wardName = selectedWards != null ? selectedWards.name : "null";

            string completeAddress = address + ", " + wardName + ", " + districtName + ", " + cityName;
            Address add = new Address();
            try
            {
                    
                add.FullNameCustomer = fullname;
                add.Phone = phone;
                add.Address1 = completeAddress;
                add.Email = email;
                PetStoreContext.Ins.Addresses.Add(add);
                await PetStoreContext.Ins.SaveChangesAsync();
                    
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Account sale = PetStoreContext.Ins.Accounts.FirstOrDefault(a => a.RoleId == 2);
            StatusOrder stt = PetStoreContext.Ins.StatusOrders.First();
            Order od = new Order();
            try
            {
                od.CreateAt = DateTime.Now;
                od.Note = note;
                od.SaleId = sale == null ? null : sale.AccountId;
                od.AccountId = acc;
                od.AddressId = add.AddressId;
                od.StatusId = stt.StatusId;
                od.PaymentMethodId = payment;
                PetStoreContext.Ins.Orders.Add(od);
                await PetStoreContext.Ins.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Redirect("/Checkout");
            }

            lsCart = PetStoreContext.Ins.ShoppingCarts.Include(p => p.Product)
                    .ThenInclude(p => p.Category).Include(a => a.Account).Where(c => c.AccountId == acc).ToList();
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            try
            {
                foreach (var pro in lsCart)
                {
                    OrderDetail odd = new OrderDetail
                    {
                        CreateAt = DateTime.Now,
                        Quantity = pro.Quantity,
                        ProductId = pro.ProductId,
                        OrderId = od.OrderId,
                        UnitPrice = pro.Product.Price,
                        Total = pro.Product.Price * pro.Quantity
                    };

                    orderDetails.Add(odd);
                }

                PetStoreContext.Ins.OrderDetails.AddRange(orderDetails);
                await PetStoreContext.Ins.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Redirect("/Checkout");
            }

            try
            {
                PetStoreContext.Ins.ShoppingCarts.RemoveRange(lsCart);
                await PetStoreContext.Ins.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Redirect("/Checkout");
            }
            string token = Guid.NewGuid().ToString();

            int totalPrice = 0;
            foreach (var p in orderDetails)
            {
                totalPrice = totalPrice + (int)p.Total;
            }
            SendMail(email, fullname, od, orderDetails, totalPrice);

            return Redirect("/Order");
        }
        private static void SendMail(string email, string cusName, Order od, List<OrderDetail> lsod, int total)
        {
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "bankingQR", "bankingQR.jpg");
            if (!System.IO.File.Exists(imagePath))
            {
                throw new FileNotFoundException("Không tìm thấy file hình ảnh!", imagePath);
            }
            string startbody = $"" +
                $"<p>Kính gửi <strong>{cusName}</strong>,</p>\r\n\r\n" +
                $"<p>Cảm ơn Quý khách đã đặt hàng tại <strong>FurFriends</strong>. Chúng tôi xin xác nhận đơn hàng của Quý khách với thông tin như sau:</p>" +
                $"\r\n\r\n<ul>" +
                $"\r\n    <li><strong>Ngày đặt hàng:</strong> {od.CreateAt}</li>\r\n   " +
                $" <li><strong>Phương thức thanh toán:</strong> {od.PaymentMethod.MethodName}</li>\r\n    " +
                $"<li><strong>Địa chỉ giao hàng:</strong> {od.Address.Address1}</li>\r\n    " +
                $"</ul>\r\n\r\n" +
                $"<p><strong>Chi tiết đơn hàng:</strong></p>\r\n<ul>\r\n    ";
            string detailOrder = "";
            foreach(var odd in lsod)
            {
                detailOrder += $"<li>Sản phẩm: {odd.Product.ProductName} - Số lượng: {odd.Quantity} - Tổng giá: {odd.Total?.ToString("N0") ?? "0"}</li>\r\n   ";
            }
            string endBody =
                $"</ul>\r\n\r\n<p>" +
                $"<strong>Tổng tiền:</strong> {total.ToString("N0")} VND</p>\r\n\r\n" +
                $"<p>Chúng tôi sẽ sớm xử lý đơn hàng và gửi cập nhật qua email. Nếu có bất kỳ thắc mắc nào, vui lòng liên hệ <strong>0396925536</strong>.</p>\r\n\r\n" +
                $"<p>Trân trọng,<br>\r\n<strong>FurFriends</strong><br>\r\n[Email Hỗ Trợ] | 0396925536</p>\r\n";

            string body = startbody + detailOrder + endBody ;
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            string emailFrom = config["InforMail:email"];
            string passFrom = config["InforMail:password"];
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(emailFrom);
            mail.To.Add(email);
            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.Subject = "Xác Nhận Thanh Toán Đơn Hàng";

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(emailFrom, passFrom),
                EnableSsl = true
            };
            smtpClient.Send(mail);
        }
    }


    public class Province
    {
        public string name { get; set; }
        public int code { get; set; }
        public string division_type { get; set; }
        public string codename { get; set; }
        public int phone_code { get; set; }
        public List<District> districts { get; set; }
    }

    public class District
    {
        public string name { get; set; }
        public int code { get; set; }
        public string division_type { get; set; }
        public string codename { get; set; }
        public int province_code { get; set; }
        public List<Ward> wards { get; set; }
    }

    public class Ward
    {
        public string name { get; set; }
        public int code { get; set; }
        public string division_type { get; set; }
        public string codename { get; set; }
        public int district_code { get; set; }
    }

    public class DistrictResponse
    {
        public List<District> districts { get; set; }
    }

    public class WardResponse
    {
        public List<Ward> wards { get; set; }
    }

}
