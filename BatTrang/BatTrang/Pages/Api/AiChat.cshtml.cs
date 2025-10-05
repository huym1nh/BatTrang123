using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BatTrang.Pages.Api
{
    // API page, không cần CSRF cho fetch JSON:
    [IgnoreAntiforgeryToken]
    public class AiChatModel : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _cfg;

        public AiChatModel(IHttpClientFactory httpFactory, IConfiguration cfg)
        {
            _httpFactory = httpFactory;
            _cfg = cfg;
        }

        // Payload client gửi lên
        public class ChatPayload
        {
            public List<Msg> Messages { get; set; } = new();
            public string? Instructions { get; set; } // tuỳ chọn: “system prompt”
        }

        public record Msg(string Role, string Content);

        public class AiReply { public string? Reply { get; set; } public string? Error { get; set; } }

        // Route: POST /api/ai-chat
        public async Task<IActionResult> OnPostAsync([FromBody] ChatPayload payload, CancellationToken ct)
        {
            if (payload?.Messages is null || payload.Messages.Count == 0)
                return BadRequest(new AiReply { Error = "messages required" });

            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                        ?? _cfg["OpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                return StatusCode(500, new AiReply { Error = "Missing OPENAI_API_KEY" });

            // Ghép input theo chuẩn Responses API:
            // Bạn có thể gửi mảng role/content trực tiếp (giống Chat), Responses API chấp nhận.
            // Thêm instructions để định giọng điệu trợ lý.
            var reqBody = new
            {
                model = "gpt-4.1-mini",
                input = payload.Messages.Select(m => new { role = m.Role, content = m.Content }).ToList(),
                instructions = payload.Instructions ?? "Bạn là trợ lý hỗ trợ khách hàng Bát Tràng, trả lời ngắn gọn, thân thiện, ưu tiên tiếng Việt.",
                store = false,
                max_output_tokens = 600
            };

            var http = _httpFactory.CreateClient();
            var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            req.Content = new StringContent(JsonSerializer.Serialize(reqBody), Encoding.UTF8, "application/json");

            using var resp = await http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
            {
                return StatusCode((int)resp.StatusCode, new AiReply { Error = body });
            }

            // Responses API có thể trả về "output_text" (tiện lợi) hoặc mảng "output" với "content".
            // Ta parse linh hoạt để tránh vỡ khi format thay đổi.
            try
            {
                using var doc = JsonDocument.Parse(body);

                // 1) Thử lấy output_text nếu có
                if (doc.RootElement.TryGetProperty("output_text", out var ot) && ot.ValueKind == JsonValueKind.String)
                {
                    return new JsonResult(new AiReply { Reply = ot.GetString() });
                }

                // 2) Fallback: output[0].content[0].text
                string? text = null;
                if (doc.RootElement.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
                {
                    foreach (var msg in output.EnumerateArray())
                    {
                        if (msg.TryGetProperty("content", out var contentArr) && contentArr.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var c in contentArr.EnumerateArray())
                            {
                                if (c.TryGetProperty("type", out var typeEl) &&
                                    typeEl.GetString() == "output_text" &&
                                    c.TryGetProperty("text", out var textEl))
                                {
                                    text = textEl.GetString();
                                    break;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(text)) break;
                    }
                }

                return new JsonResult(new AiReply { Reply = text ?? "(Không tìm thấy nội dung phản hồi phù hợp.)" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new AiReply { Error = "Parse error: " + ex.Message });
            }
        }
    }
}
