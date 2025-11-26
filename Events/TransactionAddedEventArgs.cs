using System;
using BudgetTracker.Models;

namespace BudgetTracker.Events
{
    public class TransactionAddedEventArgs : EventArgs
    {
        public Transaction NewTransaction { get; }

        public TransactionAddedEventArgs(Transaction newTransaction)
        {
            NewTransaction = newTransaction;
        }
    }
}