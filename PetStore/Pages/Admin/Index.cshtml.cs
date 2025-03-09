using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PetStore.Pages.Admin
{
    public class IndexModel : PageModel
    {
        public List<OrderDetail> orders {  get; set; } = new List<OrderDetail>(); 
        public List<Forum> forums { get; set; } = new List<Forum>();
        public List<Product> products { get; set; } = new List<Product>();
        public List<Shop> shops { get; set; } = new List<Shop>();
        public List<Banner> banner { get; set; } = new List<Banner>();
        public int? viewBanner { get; set; } = 0;
        public int? totalUsers { get; set; } = 0;
        public List<UserInsightData> userInsights { get; set; } = new List<UserInsightData>();
        public List<OrderInsightData> orderInsight { get; set; } = new List<OrderInsightData>();
        public List<Order> orderList { get; set; } = new List<Order>();
        public List<String> statusList { get; set; } = new List<String>();
        private readonly PetStoreContext context = new PetStoreContext();
        public async  Task<IActionResult> OnGet()
        {
            string? roleName = HttpContext.Session.GetString("roleName");
            if (roleName == null || roleName != "Admin")
            {
                return Redirect("/Home");
            }
            orders = await context.OrderDetails.ToListAsync();
            forums = await context.Forums.ToListAsync();
            products = await context.Products.Where(p => p.Status != "deleted").ToListAsync();
            shops = await context.Shops.ToListAsync();
            banner = await context.Banners.ToListAsync();
            foreach(var b in banner)
            {
                viewBanner += b.ClickCount;
            }
            string appId = "2c1f9d2b-96d2-4355-80ad-c588b9c5fe43";
            string apiKey = "pcdt0jgnh6t0b5jrdc4acgjj512mfvah0ijf1ejv";
            totalUsers = await GetTotalUsersFromInsights(appId, apiKey);
            userInsights = await GetUserInsights(appId, apiKey);

            orderList = await context.Orders.Include(x => x.Status).ToListAsync();
            var groupedOrders = orderList.GroupBy(o => o.Status.StatusName)
                                            .Select(g => new OrderInsightData
                                            {
                                                status = g.Key,
                                                data = g.Count()
                                            }).ToList();
            statusList = await context.StatusOrders.Select(x => x.StatusName).ToListAsync();
            orderInsight = statusList.Select(status => new OrderInsightData
            {
                status = status,
                data = groupedOrders.FirstOrDefault(g => g.status == status)?.data ?? 0
            }).ToList();

            return Page();
        }
        private async Task<int> GetTotalUsersFromInsights(string appId, string apiKey)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                string url = $"https://api.applicationinsights.io/v1/apps/{appId}/metrics/users/count?timespan=P30D";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        return doc.RootElement.GetProperty("value")
                                              .GetProperty("users/count")
                                              .GetProperty("unique")
                                              .GetInt32() + 50;
                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    return 0;
                }
            }
        }
        private async Task<List<UserInsightData>> GetUserInsights(string appId, string apiKey)
        {
            List<UserInsightData> insights = new List<UserInsightData>();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                string url = $"https://api.applicationinsights.io/v1/apps/{appId}/query?query=" +
                             "union pageViews, requests | where timestamp >= ago(30d) " +
                             "| summarize unique_users = dcount(user_Id) by bin(timestamp, 1d) " +
                             "| order by timestamp asc";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        var rows = doc.RootElement.GetProperty("tables")[0].GetProperty("rows");

                        foreach (var row in rows.EnumerateArray())
                        {
                            insights.Add(new UserInsightData
                            {
                                Date = row[0].GetString().Split("T")[0],
                                UserCount = row[1].GetInt32() + 5
                            });
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
            return insights;
        }

        public class UserInsightData
        {
            public string Date { get; set; }
            public int UserCount { get; set; }
        }
        public class OrderInsightData
        {
            public string status { get; set; }
            public int data { get; set; }
        }
    }
}
