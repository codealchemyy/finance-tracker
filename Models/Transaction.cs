namespace BudgetTracker.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public DateOnly Date { get; set; }
        public TransactionType Type { get; set; }
    }
}