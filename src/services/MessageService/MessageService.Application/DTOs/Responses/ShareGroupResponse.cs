using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.DTOs.Responses
{
    public class ShareGroupResponse
    {
        public string SharedCode { get; set; }
        public DateTime SharedExpired { get; set; }
        public string QrCodeImage { get; set; }
    }
}
