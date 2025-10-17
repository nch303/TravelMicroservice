using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.DTOs.Requests
{
    public class CreateTransactionRequest
    {
        public Guid UserId {  get; set; }
        public Guid PackageId { get; set; }
        public decimal Amount { get; set; }
    }
}
