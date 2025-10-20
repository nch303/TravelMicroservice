using PaymentService.Application.IServices;
using PaymentService.Domain.Entities;
using PaymentService.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Services
{
    public class TransactionService: ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            var newTransaction = await _transactionRepository.CreateTransactionAsync(transaction);
            return newTransaction;
        }

        public async Task<Transaction?> GetTransactionById(Guid transactionId)
        {
            var transaction = await _transactionRepository.GetTransactionById(transactionId);
            if(transaction == null)
            {
                throw new Exception("Can not find transaction");
            }

            return transaction;
        }

        public async Task SaveChangesAsync()
        {
            await _transactionRepository.SaveChangesAsync();
        }

        public async Task<List<Transaction>?> GetAllTransactionsAsync()
        {
            return await _transactionRepository.GetAllTransactionsAsync();
        }
    }
}
