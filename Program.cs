using System;
using System.ComponentModel;
using System.Linq;
using BudgetTracker.Models;
using BudgetTracker.Services;

const string DataDirectory = "data";
const string LogsDirectory = "logs";
const string LogFileName = "transactions.log";

// Create services
var storageService = new StorageService(DataDirectory);
var transactionService = new TransactionService(storageService);
var loggerService = new LoggerService(LogsDirectory, LogFileName);

// Wire up event: when a transaction is added, logger reacts
transactionService.TransactionAdded += loggerService.HandleTransactionAdded;

bool exitRequested = false;

while (!exitRequested)
{
    Console.WriteLine();
    Console.WriteLine("=== Budget Tracker ===");
    Console.WriteLine("1) Add transaction");
    Console.WriteLine("2) Remove transaction");
    Console.WriteLine("3) Generate report");
    Console.WriteLine("0) Exit");
    Console.Write("Choose an option: ");

    var choice = Console.ReadLine();
    Console.WriteLine();
    switch (choice)
    {
        case "1":
            AddTransactionFlow(transactionService);
            break;

        case "2":
            RemoveTransactionFlow(transactionService);
            break;

        case "3":
            GenerateReportFlow(transactionService);
            break;

        case "0":
            exitRequested = true;
            Console.WriteLine("Goodbye!");
            break;

        default:
            Console.WriteLine("Invalid choice. Please select 1, 2, 3 or 0.");
            break;
    }
}

// ---- local helper functions ----

void AddTransactionFlow(TransactionService transactionService)
{
    Console.WriteLine("=== Add Transaction ===");

    // 1) Ask for type
    TransactionType type;
    while (true)
    {
        Console.Write("Select type (1 = Income, 2 = Expense): ");
        var typeInput = Console.ReadLine();

        if (typeInput == "1")
        {
            type = TransactionType.Income;
            break;
        }

        if (typeInput == "2")
        {
            type = TransactionType.Expense;
            break;
        }

        Console.WriteLine("Invalid choice. Please enter 1 for Income or 2 for Expense.");
    }

    // 2) Ask for description
    Console.Write("Enter description: ");
    var description = Console.ReadLine() ?? string.Empty;

    // 3) Ask for amount (with validation)
    decimal amount;
    while (true)
    {
        Console.Write("Enter amount: ");
        var amountInput = Console.ReadLine();

        if (decimal.TryParse(amountInput, out amount) && amount > 0)
        {
            break;
        }

        Console.WriteLine("Please enter a valid positive number for the amount.");
    }

    try
    {

        // 4) Call the service (ADD)
        var transaction = transactionService.AddTransaction(type, description, amount);

        Console.WriteLine();
        Console.WriteLine("Transaction added successfully!");
        Console.WriteLine($"Id:          {transaction.Id}");
        Console.WriteLine($"Type:        {transaction.Type}");
        Console.WriteLine($"Description: {transaction.Description}");
        Console.WriteLine($"Amount:      {transaction.Amount}");
        Console.WriteLine($"Date:        {transaction.Date}");
        Console.WriteLine();
        Console.WriteLine("JSON updated in data/ and log written to logs/transactions.log.");

    }
    catch (Exception)
    {
        Console.WriteLine("An error occurred while adding the transaction. Please try again.");
    }

}

void RemoveTransactionFlow(TransactionService transactionService)
{
    Console.WriteLine("=== Remove Transaction ===");
    Console.Write("Enter the Id of the transaction to remove: ");
    var idInput = Console.ReadLine();

    if (!Guid.TryParse(idInput, out var id))
    {
        Console.WriteLine("The Id you entered is not a valid GUID.");
        return;
    }

    try
    {
        

        var removed = transactionService.RemoveTransaction(id);

        if (removed)
        {
            Console.WriteLine("Transaction removed successfully.");
        }
        else
        {
            Console.WriteLine("No transaction found with that Id.");
        }
    }
    catch (Exception)
    {
        Console.WriteLine("An error occurred while removing the transaction. Please try again.");
    }
}

void GenerateReportFlow (TransactionService transactionService)

{
    Console.WriteLine("=== Generate Report ===");

    Console.Write("Enter start date (YYYY-MM-DD): ");
    var startDateInput = Console.ReadLine();

    Console.Write("Enter end date (YYYY-MM-DD): ");
    var endDateInput = Console.ReadLine();

    if (!DateOnly.TryParse(startDateInput, out var startDate) ||
        !DateOnly.TryParse(endDateInput, out var endDate))
    {
        Console.WriteLine("Please enter valid dates in the format YYYY-MM-DD.");
        return;
    }

    if (endDate < startDate)
    {
        Console.WriteLine("End date cannot be before start date.");
        return;
    }

    try
    {

        var transactions = transactionService.GetTransactionsInRange(startDate, endDate).ToList();

        if (transactions.Count == 0)
        {
            Console.WriteLine("No transactions found in this specific range.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Filter options:");
        Console.WriteLine("1) All transactions");
        Console.WriteLine("2) Income only");
        Console.WriteLine("3) Expense only");
        Console.Write("Choose filter: ");
        var filterChoice = Console.ReadLine();

        // Start with the full set, then apply optional filters
        var filtered = transactions.AsEnumerable();

        switch (filterChoice)
        {
            case "2":
                filtered = filtered.Where(t => t.Type == TransactionType.Income);
                break;
            case "3":
                filtered = filtered.Where(t => t.Type == TransactionType.Expense);
                break;
            case "1":
            default:
                // All transactions; no extra filter
                break;
        }

        var filteredList = filtered.ToList();

        if (filteredList.Count == 0)
        {
            Console.WriteLine("No transactions match the selected filter.");
            return;
        }

        // LINQ to calculate totals

        var totalIncome = filteredList
        .Where(t => t.Type == TransactionType.Income)
        .Sum(t => t.Amount);

        var totalExpense = filteredList
        .Where(t => t.Type == TransactionType.Expense)
        .Sum(t => t.Amount);

        var netTotal = totalIncome - totalExpense;

        Console.WriteLine();
        Console.WriteLine($"Report from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
        Console.WriteLine($"Number of transactions: {filteredList.Count}");
        Console.WriteLine($"Total income:   {totalIncome}");
        Console.WriteLine($"Total expenses: {totalExpense}");
        Console.WriteLine($"Net balance:    {netTotal}");
    }
    catch (Exception)
    {
        Console.WriteLine("An error occurred while generating the report. Please try again.");
    }
}