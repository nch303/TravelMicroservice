using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Domain.IRepositories;
using PaymentService.Infrastructure.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.Repositories
{
    public class TransactionRepository: ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            var entity = await _context.Transactions.AddAsync(transaction);   
            await _context.SaveChangesAsync();
            return entity.Entity;
        }

        public async Task<Transaction?> GetTransactionById(Guid transactionId)
        {
            return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Transaction>?> GetAllTransactionsAsync()
        {
            return await _context.Transactions.ToListAsync();
        } 

        public async  Task<List<Transaction>?> GetByCurrentAccountAsync(Guid accountId)
        {
            return await _context.Transactions.Where(t => t.UserId == accountId).ToListAsync();
        }
    }
}
