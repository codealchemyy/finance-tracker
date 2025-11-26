using System;
using System.Collections.Generic;
using BudgetTracker.Events;
using BudgetTracker.Models;

namespace BudgetTracker.Services
{
    public class TransactionService
    {
        private readonly StorageService _storageService;

        public event EventHandler<TransactionAddedEventArgs>? TransactionAdded;

        public TransactionService(StorageService storageService)
        {
            _storageService = storageService;
        }

        public Transaction AddTransaction(TransactionType type, string description, decimal amount)
        {
            var now = DateTime.Now;
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Timestamp = now,
                Date = DateOnly.FromDateTime(now),
                Type = type,
                Description = description,
                Amount = amount
            };

            // Save transaction using storage service (to the correct daily JSON file)
            _storageService.SaveTransaction(transaction);

            //Notify listeners (e.g. LoggerService) that a transaction was added
            OnTransactionAdded(transaction);
            return transaction;
        }

        public bool RemoveTransaction(Guid id)
        {
            return _storageService.RemoveTransaction(id);
        }

        public IEnumerable<Transaction> GetTransactionsInRange(DateOnly startDate, DateOnly endDate)
        {
            return _storageService.GetTransactionsInRange(startDate, endDate);
        }

        private void OnTransactionAdded(Transaction newTransaction)
        {
            TransactionAdded?.Invoke(this, new TransactionAddedEventArgs(newTransaction));
        }
    }
}
