using Microsoft.AspNetCore.Http;
using Mscc.GenerativeAI; // Đảm bảo using này
using ScheduleService.Application.DTOs.Requests;
using ScheduleService.Application.Helpers;
using ScheduleService.Application.IServices;
using System.Collections.Generic; // Cần cho List
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScheduleService.Application.Services
{
    public class TravelAIChatService : ITravelAIChatService
    {
        private readonly GenerativeModel _model;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string ChatHistoryKey = "TravelChatHistory";

        public TravelAIChatService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            var googleAI = new GoogleAI(apiKey);

            var systemInstruction = new Content(
                "Bạn là một trợ lý du lịch AI chuyên nghiệp và hữu ích. Bạn chuyên tư vấn, trả lời các câu hỏi và tạo lịch trình du lịch cho người dùng. Luôn luôn trả lời bằng tiếng Việt."
            );

            // 1. CÁCH NÀY LÀ ĐÚNG:
            //    systemInstruction được gán khi tạo Model
            _model = googleAI.GenerativeModel(
                model: Model.Gemini25Flash,
                systemInstruction: systemInstruction
            );
        }

        public async Task<object> GetResponseAsync(string userMessage)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var history = session.GetFromJson<List<ContentResponse>>(ChatHistoryKey) ?? new();

            var chatSession = _model.StartChat(history);

            // Prompt yêu cầu Gemini trả về 3 phần: mô tả + schedule + activities
            var response = await chatSession.SendMessage($@"
Người dùng nói: ""{userMessage}"".

Bạn là trợ lý du lịch thông minh.
Hãy trả về thông tin gồm **3 phần** sau bằng tiếng Việt:

1️⃣ **Phần mô tả tự nhiên:**
Giới thiệu chi tiết hành trình du lịch, gợi ý địa điểm nổi bật, và lời khuyên hữu ích.

2️⃣ **Phần JSON đầu tiên** (lịch trình tổng quan - CreateScheduleRequest), theo định dạng:
{{
  ""sharedCode"": ""string"", (mặc định là null)
  ""title"": ""string"",
  ""startLocation"": ""string"",
  ""destination"": ""string"",
  ""startDate"": ""yyyy-MM-dd"",
  ""endDate"": ""yyyy-MM-dd"",
  ""participantsCount"": 1,(là số người tham gia hiện tại của nhóm nên khi tạo mặc định là 1)
  ""notes"": ""string"",
  ""isShared"": bool
}}

3️⃣ **Phần JSON thứ hai** (danh sách hoạt động - List<CreateScheduleActivityRequest>), theo định dạng:
[
  {{
    ""placeName"": ""string"",
    ""location"": ""string"",
    ""latitude"": ""string?"",
    ""longitude"": ""string?"",
    ""description"": ""string"",
    ""checkInTime"": ""yyyy-MM-ddTHH:mm:ss"",
    ""checkOutTime"": ""yyyy-MM-ddTHH:mm:ss"",
    ""orderIndex"": int,(orderIndex là thứ tự các hoạt động trong 1 ngày, mỗi này sẽ bắt đầu từ số 1)
    ""scheduleId"": ""00000000-0000-0000-0000-000000000000""
  }}
]

⚠️ Ghi chú:
- Không thêm văn bản nào khác bên trong JSON.
- Nếu không biết tọa độ, để null.
- `scheduleId` tạm thời đặt giá trị mặc định (toàn 0).
- participantCount (là số người tham gia hiện tại của nhóm nên khi tạo mặc định là 1)
- sharedCode mặc định là null
");

            session.SetAsJson(ChatHistoryKey, chatSession.History);

            string aiText = response.Text;
            CreateScheduleRequest? schedule = null;
            List<CreateScheduleActivityRequest>? activities = null;

            try
            {
                // Tìm hai phần JSON trong nội dung AI trả về
                var jsonMatches = Regex.Matches(aiText, @"\{[\s\S]*?\}|\[[\s\S]*?\]");

                if (jsonMatches.Count >= 2)
                {
                    var scheduleJson = jsonMatches[0].Value;
                    var activitiesJson = jsonMatches[1].Value;

                    schedule = JsonSerializer.Deserialize<CreateScheduleRequest>(scheduleJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    activities = JsonSerializer.Deserialize<List<CreateScheduleActivityRequest>>(activitiesJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"❌ Lỗi parse JSON từ AI: {ex.Message}");
            }

            // Trả về 3 phần trong response JSON
            var result = new
            {
                success = true,
                message = "AI phản hồi thành công.",
                timestamp = DateTime.UtcNow,
                aiMessage = aiText,
                scheduleData = schedule,
                activitiesData = activities
            };

            return result;
        }



        public void ClearChatHistory()
        {
            _httpContextAccessor.HttpContext.Session.Remove(ChatHistoryKey);
        }
    }
}