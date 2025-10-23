using Microsoft.AspNetCore.Http;
using Mscc.GenerativeAI; // Đảm bảo using này
using ScheduleService.Application.Helpers;
using ScheduleService.Application.IServices;
using System.Threading.Tasks;
using System.Collections.Generic; // Cần cho List
using System.Linq;

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

        public async Task<string> GetResponseAsync(string userMessage)
        {
            var session = _httpContextAccessor.HttpContext.Session;

            // 2. SỬA LỖI Ở ĐÂY:
            //    Lấy lịch sử ra với đúng kiểu List<ContentResponse>
            var history = session.GetFromJson<List<ContentResponse>>(ChatHistoryKey);

            // 3. SỬA LỖI Ở ĐÂY:
            //    Nếu không có, tạo một List<ContentResponse> mới
            if (history == null)
            {
                history = new List<ContentResponse>();
            }

            // 4. SỬA LỖI Ở ĐÂY:
            //    Xóa tham số "history:" đi, chỉ cần truyền biến 'history'
            //    Bây giờ 'history' là List<ContentResponse> nên sẽ khớp
            var chatSession = _model.StartChat(history);

            // 5. Gửi tin nhắn (đã sửa lỗi chính tả 'SendMessage')
            var response = await chatSession.SendMessage(userMessage);

            // 6. Lưu lại chatSession.History (vốn là List<ContentResponse>)
            session.SetAsJson(ChatHistoryKey, chatSession.History);

            return response.Text;
        }

        public void ClearChatHistory()
        {
            _httpContextAccessor.HttpContext.Session.Remove(ChatHistoryKey);
        }
    }
}