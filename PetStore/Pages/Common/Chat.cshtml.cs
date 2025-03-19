using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PetStore.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PetStore.Pages.Common
{
    [IgnoreAntiforgeryToken]
    public class ChatModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _cacheKey = "UserRequestCount_" + DateTime.UtcNow.ToString("yyyyMMdd");
        private readonly IMemoryCache _cache;
        private readonly PetStoreContext context = new PetStoreContext();

        public ChatModel(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;

        }
        public IActionResult OnGet() { return Redirect("/Home"); }
        public async Task<IActionResult> OnPost()
        {
            var requestCount = _cache.GetOrCreate(_cacheKey, entry =>
            {
                entry.AbsoluteExpiration = DateTime.UtcNow.Date.AddDays(1);
                return 0;
            });
            if (requestCount >= 30)
            {
                var errorResponse = new
                {
                    id = Guid.NewGuid().ToString(),
                    @object = "chat.completion",
                    created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    model = "gpt-4o",
                    choices = new[]
                    {
                        new
                        {
                            index = 0,
                            message = new { role = "assistant", content = "Bạn đã vượt quá số request tối đa trong ngày." },
                            finish_reason = "limit_exceeded"
                        }
                    },
                    usage = new { prompt_tokens = 0, completion_tokens = 0, total_tokens = 0 }
                };

                return Content(JsonSerializer.Serialize(errorResponse), "application/json");
            }

            _cache.Set(_cacheKey, requestCount + 1);

            var shops = await context.Shops.ToListAsync();
            var dataShops = new
            {
                Title = "Shop cộng tác",
                Shop = shops
            };

            var product = await context.Products.Where(x => x.Status != "deleted").Take(10).ToListAsync();
            var dataProduct = new
            {
                Title = "Sản Phẩm",
                Product = product
            };

            var teamData = new
            {
                Title = "Thành Viên Nhóm",
                TeamMembers = new List<object>
                {
                    new { Name = "Hà Bảo Khánh", Class = "IA1704-AS", Group = "Group 1" },
                    new { Name = "Hồ Nhã Uyên", Class = "IA1704-AS", Group = "Group 1" },
                    new { Name = "Tạ Duy Hải", Class = "IA1704-AS", Group = "Group 1" },
                    new { Name = "Phan Anh Quân", Class = "IA1704-AS", Group = "Group 1" },
                    new { Name = "Nguyễn Việt Hoàng", Class = "IA1704-AS", Group = "Group 1" },
                    new { Name = "Nguyễn Tiến Đạt", Class = "IA1704-AS", Group = "Group 1" }
                }
            };

            var contextData = new
            {
                Shops = dataShops,
                Products = dataProduct,
                Team = teamData
            };
            string jsonContextData = JsonSerializer.Serialize(contextData, new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });

            string _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();

            var chatRequest = JsonSerializer.Deserialize<ChatRequest>(body);
            if (chatRequest == null || string.IsNullOrEmpty(chatRequest.UserMessage))
                return BadRequest("Message cannot be empty");

            string modifiedUserMessage = $"Dưới đây là dữ liệu của tôi:\n{jsonContextData}\n\nCâu hỏi của tôi: {chatRequest.UserMessage}\n\n" +
            "Hãy trả lời câu hỏi trên cho tôi nếu như data trả lời có nằm trong data tôi đã cho (lưu ý, hãy trả lời một cách tự nhiên mà không để lộ rằng dữ liệu đã được cung cấp sẵn)" +
            ", nếu không hãy trả lời tùy biến với thông tin chính xác.";

            var httpClient = _httpClientFactory.CreateClient();
            var requestData = new
            {
                model = "gpt-4o",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = modifiedUserMessage }
                },
                max_tokens = 1000,
                temperature = 0.7
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            await Task.Delay(5000);
            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Error calling OpenAI API");

            var responseString = await response.Content.ReadAsStringAsync();
            return Content(responseString, "application/json");
        }
    }
    public class ChatRequest
    {
        public string UserMessage { get; set; }
    }
}
