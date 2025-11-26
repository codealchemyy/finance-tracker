using System;
using System.IO;
using BudgetTracker.Events;

namespace BudgetTracker.Services
{
    public class LoggerService
    {
        private readonly string _logsDirectory;
        private readonly string _logFileName;

        public LoggerService(string logsDirectory, string logFileName)
        {
            _logsDirectory = logsDirectory;
            _logFileName = logFileName;
        }
        
        public void HandleTransactionAdded(object? sender, TransactionAddedEventArgs e)
        {
            try 
            {
                // Make sure the logs directory exists
                Directory.CreateDirectory(_logsDirectory);

                var filePath = Path.Combine(_logsDirectory, _logFileName);
                var t = e.NewTransaction;

                var line = $"{t.Timestamp:yyyy-MM-dd HH:mm:ss} | {t.Type} | {t.Description} | {t.Amount:F2} | Id: {t.Id}{Environment.NewLine}";

                // Append the lone to the log file
                File.AppendAllText(filePath, line);
            }
            catch (Exception ex)
            {
                // In a real-world app, consider more robust error handling
                Console.WriteLine($"Failed to log transaction: {ex.Message}");
            }
        }
    }
}


/* “Think of it like: I am the historian. For now I’m just holding a notebook. Later, I’ll start writing in it.” */