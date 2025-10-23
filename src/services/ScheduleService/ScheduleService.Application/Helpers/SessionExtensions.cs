using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScheduleService.Application.Helpers
{
    public static class SessionExtensions
    {
        // Phương thức để LƯU đối tượng vào Session dưới dạng JSON
        public static void SetAsJson<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Phương thức để LẤY đối tượng từ Session (chuyển đổi từ JSON)
        public static T? GetFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
