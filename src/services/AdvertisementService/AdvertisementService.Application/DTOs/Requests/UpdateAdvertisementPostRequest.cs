using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Application.DTOs.Requests
{
    public class UpdateAdvertisementPostRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid? PackagePurchaseId { get; set; }

        // Danh sách media cũ muốn giữ lại
        public List<Guid>? KeepMediaIds { get; set; }

        // File media mới (ảnh/video)
        public List<IFormFile>? NewMediaFiles { get; set; }
        public List<int>? NewMediaTypes { get; set; } // 0 = Image, 1 = Video
    }
}
