using AdvertisementService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Domain.Entities
{
    public class AdvertisementMedia
    {
        public Guid Id { get; set; }
        public string MediaUrl { get; set; }
        public MediaType MediaType { get; set; } // e.g., "image", "video"
        public DateTime UploadedAt { get; set; }


        // Navigation property to link to the AdvertisementPost
        public Guid AdvertisementPostId { get; set; }
        public AdvertisementPost AdvertisementPost { get; set; }
    }
}
