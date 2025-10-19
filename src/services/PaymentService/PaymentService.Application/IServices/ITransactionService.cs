using PaymentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.IServices
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Task<Transaction?> GetTransactionById(Guid transactionId);
        Task SaveChangesAsync();
        Task<List<Transaction>?> GetAllTransactionsAsync();
    }
}
