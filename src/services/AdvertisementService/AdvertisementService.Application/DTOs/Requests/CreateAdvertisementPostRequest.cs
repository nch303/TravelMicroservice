using AdvertisementService.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Application.DTOs.Requests
{
    public class CreateAdvertisementPostRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid PackagePurchaseId { get; set; }

        // Danh sách file upload
        public List<IFormFile> MediaFiles { get; set; } = new();

        // Loại file tương ứng: Image, Video,...
        [Required(ErrorMessage = "Vui lòng chọn loại media cho từng file (0 -> Image, 1 -> Video")]
        public List<int> MediaTypes { get; set; } = new();
    }
}
