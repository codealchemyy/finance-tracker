# Budget Tracker

A small C#/.NET console app that tracks incomes and expenses, stores them as daily JSON files, and generates date-range reports using LINQ. Built as a solo project to practice file I/O, JSON serialization, events, and basic error handling.

---

## Features

- Add transactions (Income or Expense)
- Store data per day in `data/YYYY-MM-DD.json`
- Remove transactions by their unique Id
- Generate reports for a date range:
  - All transactions, Income only, or Expense only
  - Total income, total expenses, net balance
- Log each added transaction to `logs/transactions.log`
- Basic error handling:
  - Invalid user input
  - Missing or corrupted data files (skips bad files instead of crashing)

---

## Requirements

- .NET SDK 9.0 (or compatible)
- Console / terminal to run the app

---

## Getting Started

### 1️⃣ Restore & build

```bash
dotnet restore
dotnet build
````

### 2️⃣ Run the app

```bash
dotnet run
```

---

## Usage

When the app starts, you see this menu:

```text
=== Budget Tracker ===
1) Add transaction
2) Remove transaction
3) Generate report
0) Exit
Choose an option:
```

### ➕ Add transaction

* Choose type:

  * `1` = Income
  * `2` = Expense
* Enter description
* Enter amount

The app will:

* Create a `Transaction` with Id, timestamp, date, type, description, amount
* Append it to `data/YYYY-MM-DD.json`
* Log the add event to `logs/transactions.log`
* Show a confirmation with full details

---

### ➖ Remove transaction

* Enter the Id (GUID) of an existing transaction

The app:

* Searches all JSON files inside `data/`
* Removes the matching transaction if found
* Shows success or “not found”

---

### 📊 Generate report

* Enter a **start** and **end** date (`YYYY-MM-DD`)
* Filter option:

  * `1` = All
  * `2` = Income only
  * `3` = Expense only

The app uses **LINQ** to calculate:

* Number of transactions
* Total income
* Total expenses
* Net balance (income − expenses)

And prints the results to the console ✨

---

## Data & Logs

| Type                      | Location                | Notes                          |
| ------------------------- | ----------------------- | ------------------------------ |
| Daily transaction storage | `data/YYYY-MM-DD.json`  | JSON array of `Transaction`    |
| Logs                      | `logs/transactions.log` | One line per added transaction |

Both directories are created automatically if missing.

---

## Architecture Overview

```
Program.cs (entry point / console menu)
│
├── Models/
│   ├── Transaction.cs
│   └── TransactionType.cs (enum)
│
├── Events/
│   └── TransactionAddedEventArgs.cs
│
└── Services/
    ├── StorageService.cs
    ├── TransactionService.cs
    └── LoggerService.cs
```

### Responsibilities

| Component                     | Responsibility                                 |
| ----------------------------- | ---------------------------------------------- |
| **Program.cs**                | User interaction, menu, calls services         |
| **StorageService**            | Read/write JSON, handle date-based files       |
| **TransactionService**        | Business logic, add/remove/query, raises event |
| **LoggerService**             | Subscribes to events, writes logs              |
| **TransactionAddedEventArgs** | Carries transaction data to event listeners    |

---

## Error Handling

* Invalid input → user can re-enter a valid value
* Corrupted JSON → skipped automatically
* Unexpected errors → caught & friendly message displayed
* App never crashes on user mistakes 🙌

---

## Author & Purpose

Solo project by **Büşra Demirhan**
Created during the WBS Coding School Software Engineering course
Purpose: strengthen .NET, file I/O, JSON, events, and clean architecture skills
