using PaymentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Domain.IRepositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Task<Transaction?> GetTransactionById(Guid transactionId);
        Task SaveChangesAsync();
        Task<List<Transaction>?> GetAllTransactionsAsync();
    }
}
