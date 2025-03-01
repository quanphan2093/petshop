using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetStore.Pages.Common
{
    [IgnoreAntiforgeryToken]
    public class ChatModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _cacheKey = "UserRequestCount_" + DateTime.UtcNow.ToString("yyyyMMdd");
        private readonly IMemoryCache _cache;

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
            if (requestCount >= 20)
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
            string _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();

            var chatRequest = JsonSerializer.Deserialize<ChatRequest>(body);
            if (chatRequest == null || string.IsNullOrEmpty(chatRequest.UserMessage))
                return BadRequest("Message cannot be empty");

            var httpClient = _httpClientFactory.CreateClient();
            var requestData = new
            {
                model = "gpt-4o",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = chatRequest.UserMessage }
                },
                max_tokens = 1000,
                temperature = 0.7
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

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
