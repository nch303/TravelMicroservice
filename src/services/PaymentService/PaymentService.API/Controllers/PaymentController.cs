using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Application.DTOs.Requests;
using PaymentService.Application.IServiceClients;
using PaymentService.Application.IServices;
using PaymentService.Domain.Entities;

namespace PaymentService.API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IAdvertisementServiceClient _advertisementServiceClient;

        public PaymentController(ITransactionService transactionService, IAuthServiceClient authServiceClient
            , IAdvertisementServiceClient advertisementServiceClient)
        {
            _transactionService = transactionService;
            _authServiceClient = authServiceClient;
            _advertisementServiceClient = advertisementServiceClient;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] CreateTransactionRequest request)
        {
            try
            {
                var currentUser = await _authServiceClient.GetCurrentAccountAsync();
                if (currentUser == null)
                {
                    throw new UnauthorizedAccessException();
                }

                var package = await _advertisementServiceClient.GetPackageByIdAsync(request.PackageId);
                if (package == null)
                {
                    throw new Exception("Can not find package");
                }

                var transactionId = Guid.NewGuid();
                var transaction = new Transaction
                {
                    Id = transactionId,
                    UserId = currentUser.Id,
                    PackageId = package.Id,
                    AmountIn = request.Amount,
                    TransactionContent = $"Pay{transactionId.ToString()}ment", // mã nội dung riêng
                    Status = "Pending"
                };



                await _transactionService.CreateTransactionAsync(transaction);

                var url = $"https://qr.sepay.vn/img?acc=0888294028&bank=VPBank&amount={request.Amount}&des={transaction.TransactionContent}";

                // Hướng dẫn người dùng chuyển tiền
                return Ok(new
                {
                    message = "Vui lòng chuyển khoản theo hướng dẫn",
                    bank = "VP Bank - 0888294028 - SEPAY COMPANY",
                    transactionId = transactionId,
                    content = transaction.TransactionContent,
                    amount = request.Amount,
                    url = url
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] SepayWebhookRequest request, [FromHeader(Name = "Authorization")] string apiKey)
        {
            try
            {
                var secretKey = Environment.GetEnvironmentVariable("SEPAY_API_KEY");
                if (apiKey != $"Apikey {secretKey}")
                {
                    return Unauthorized("Invalid API Key");
                }

                if (request.TransferType != "in")
                    return Ok("Ignore: outgoing transaction");


                // Tách theo dấu '-'
                int startIndex = request.Content!.IndexOf("Pay") + "Pay".Length;
                int endIndex = request.Content.IndexOf("ment");
                var id = new Guid();

                if (startIndex >= 0 && endIndex > startIndex)
                {
                    string guidString = request.Content.Substring(startIndex, endIndex - startIndex).Trim();
                    Console.WriteLine(guidString);

                    // Nếu cần ép về kiểu Guid
                    if (Guid.TryParse(guidString, out Guid transactionId))
                    {
                        id = transactionId;
                        Console.WriteLine($"✅ GUID hợp lệ: {transactionId}");
                    }
                    else
                    {
                        Console.WriteLine("❌ Không phải GUID hợp lệ");
                    }
                }

                // Tìm transaction trùng nội dung chuyển khoản
                var transaction = await _transactionService.GetTransactionById(id);

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
                    var purchaseRequest = new CreatePurchaseRequest
                    {
                        UserId = transaction.UserId!.Value,
                        PackageId = transaction.PackageId!.Value,
                        TransactionId = transaction.Id
                    };

                    await _advertisementServiceClient.CreatePurchaseAsync(purchaseRequest);
                }
                

                return Ok("Webhook processed");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("transaction/status/by-id")]
        public async Task<IActionResult> GetTransactionById(Guid transactionId)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionById(transactionId);
                return Ok(new { status = transaction!.Status });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("transaction/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTransactionAsync()
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
