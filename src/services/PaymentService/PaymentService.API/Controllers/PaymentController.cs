using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Application.DTOs.Requests;
using PaymentService.Application.IServices;
using PaymentService.Domain.Entities;

namespace PaymentService.API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public PaymentController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreateTransactionRequest request)
        {
            // request: { userId, packageId, amount, note }

            var transactionId = Guid.NewGuid();
            var transaction = new Transaction
            {
                Id = transactionId,
                UserId = request.UserId,
                PackageId = request.PackageId,
                AmountIn = request.Amount,
                TransactionContent = $"{transactionId.ToString()}", // mã nội dung riêng
                Status = "Pending"
            };
                
            await _transactionService.CreateTransactionAsync(transaction);

            var url = $"https://qr.sepay.vn/img?acc=0888294028&bank=VPBank&amount={request.Amount}&des={transaction.Id}";

            // Hướng dẫn người dùng chuyển tiền
            return Ok(new
            {
                message = "Vui lòng chuyển khoản theo hướng dẫn",
                bank = "VP Bank - 0888294028 - SEPAY COMPANY",
                content = transaction.TransactionContent,
                amount = request.Amount,
                url = url
            });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] SepayWebhookRequest request, [FromHeader(Name = "Authorization")] string apiKey)
        {
            var secretKey = Environment.GetEnvironmentVariable("SEPAY_API_KEY");
            if (apiKey != $"Apikey {secretKey}")
            {
                return Unauthorized("Invalid API Key");
            }

            if (request.TransferType != "in")
                return Ok("Ignore: outgoing transaction");

            var transactionId = Guid.Parse(request.Content);

            // Tìm transaction trùng nội dung chuyển khoản
            var transaction = await _transactionService.GetTransactionById(transactionId);

            if (transaction != null && transaction.Status == "Pending")
            {
                transaction.Status = "Success";
                transaction.TransactionDate = DateTime.Parse(request.TransactionDate);
                transaction.AccountNumber = request.AccountNumber;
                transaction.SubAccount = request.SubAccount;
                transaction.AmountIn = request.TransferAmount;
                transaction.Accumulated = request.Accumulated;
                transaction.Gateway = request.Gateway;
                transaction.Code = request.Code;
                await _transactionService.SaveChangesAsync();

                // Kích hoạt package cho user
                //var package = await _context.Packages.FindAsync(transaction.PackageId);
                //if (package != null)
                //{
                //    // ví dụ cập nhật trạng thái hoặc cộng lượt đăng bài
                //    // package.RemainingPosts += package.PostCount;
                //}
            }

            return Ok("Webhook processed");
        }
    }
}
