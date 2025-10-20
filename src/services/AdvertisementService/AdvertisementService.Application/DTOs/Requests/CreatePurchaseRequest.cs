using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Application.DTOs.Requests
{
    public class CreatePurchaseRequest
    {
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
