using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using BudgetTracker.Models;

namespace BudgetTracker.Services
{
    public class StorageService
    {
        private readonly string _dataDirectory;
        public StorageService(string dataDirectory)
        {
            _dataDirectory = dataDirectory;
        }

        public void SaveTransaction(Transaction transaction)
        {
            // Ensure data directory exists
            Directory.CreateDirectory(_dataDirectory);

            // One file per day: data/YYYY-MM-DD.json
            var fileName = $"{transaction.Date:yyyy-MM-dd}.json";
            var filePath = Path.Combine(_dataDirectory, fileName);

            List<Transaction> transactions;

            if (File.Exists(filePath))
            {
                // Load existing transactions
                var json = File.ReadAllText(filePath);

                if(string.IsNullOrWhiteSpace(json))
                {
                    transactions = new List<Transaction>();
                }
                else
                {
                    var existing = JsonSerializer.Deserialize<List<Transaction>>(json);
                    transactions = existing ?? new List<Transaction>();
                }

            }
            else
            {
                transactions = new List<Transaction>();
            }

            // Add new transaction
            transactions.Add(transaction);

            // Save back to file
            var updatedJson = JsonSerializer.Serialize(
                transactions,
                new JsonSerializerOptions { WriteIndented = true}
            );

            File.WriteAllText(filePath, updatedJson);
        }

        public bool RemoveTransaction(Guid Id)
        {
            //Make sure data directory exists
            Directory.CreateDirectory(_dataDirectory);

            // Look through all JSON files in data/
            var jsonFiles = Directory.GetFiles(_dataDirectory, "*.json");

            foreach (var filePath in jsonFiles)
            {
                var json = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    continue;
                }

                List<Transaction> transactions;

                try
                {
                    // JsonSerializer.Deserialize can return null, ensure we always have a non-null list
                    transactions = JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
                }

                catch
                {
                    // if a file is correct, skip it
                    continue;
                }

                if (transactions == null || transactions.Count == 0)
                {
                    continue;
                }

                var index = transactions.FindIndex(t => t.Id == Id);
                if (index == -1)
                {
                    continue;
                }

                // fount it -> remove and rewrite file
                transactions.RemoveAt(index);

                var updatedJson = JsonSerializer.Serialize(
                    transactions,
                    new JsonSerializerOptions { WriteIndented = true }
                );

                File.WriteAllText(filePath, updatedJson);

                return true; // removed
            }

            return false; // not found
        }

        public IEnumerable<Transaction> GetTransactionsInRange(DateOnly startDate, DateOnly endDate)
        {
            var results = new List<Transaction>();

            // Make sure data directory exists
            Directory.CreateDirectory(_dataDirectory);

            var jsonFiles = Directory.GetFiles(_dataDirectory, "*.json");

            foreach (var filePath in jsonFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath); // e.g. "2024-06-15"

                if (!DateOnly.TryParseExact(
                    fileName,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var fileDate))
                {
                    continue; // skip files that don't match the date format
                }

                if(fileDate < startDate || fileDate > endDate)
                {
                    continue; // skip files outside the date range
                }

                var json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    continue;
                }

                List<Transaction> transactions;

                try
                {
                    transactions = JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
                }
                catch
                {
                    continue; // skip files that can't be deserialized
                }

                if (transactions is null || transactions.Count == 0)
                {
                    continue;
                }

                results.AddRange(transactions);
            }

            return results;
        }
    }
}