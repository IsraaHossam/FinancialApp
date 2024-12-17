using ImperativeFinancialApp.Data;
using ImperativeFinancialApp.Models;
using ImperativeFinancialApp.UI;

namespace ImperativeFinancialApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string dataFilePath = "./Data/financialData.json";
            var jsonOptions = JsonHelper.GetJsonOptions();

            // Load existing data
            var financialData = DataStorage.LoadData(dataFilePath, jsonOptions);

            bool running = true;

            while (running)
            {
                UserInterface.ShowMainMenu();

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTransaction(financialData);
                        break;
                    case "2":
                        ImportTransactions(financialData);
                        break;
                    case "3":
                        UserInterface.ViewTransactions(financialData.Transactions);
                        break;
                    case "4":
                        SetBudget(financialData);
                        break;
                    case "5":
                        UserInterface.ViewBudgets(financialData);
                        break;
                    case "6":
                        AddGoal(financialData);
                        break;
                    case "7":
                        ViewFinancialAnalytics(financialData);
                        break;
                    case "8":
                        ExportFinancialAnalytics(financialData);
                        break;
                    case "9":
                        running = false;
                        Console.WriteLine("Exiting the application. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                DataStorage.SaveData(financialData, "./Data", "financialData.json", jsonOptions);
            }
        }

        static void AddTransaction(FinancialData financialData)
{
    try
    {
        var date = InputHelper.GetDateInput("Date (yyyy-MM-dd): ");
        var category = InputHelper.GetNonEmptyInput("Category: ");
        var amount = InputHelper.GetDecimalInput("Amount: ");
        var type = InputHelper.GetNonEmptyInput("Type (income/expense): ").ToLower();

        while (!BusinessLogic.IsIncome(type) && !BusinessLogic.IsExpense(type))
        {
            Console.WriteLine("Invalid transaction type. Must be 'income' or 'expense'.");
            type = InputHelper.GetNonEmptyInput("Type (income/expense): ").ToLower();
        }

        financialData.Transactions.Add(new Transaction
        {
            Date = date,
            Category = category,
            Amount = amount,
            Type = type
        });

        Console.WriteLine("Transaction added successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error adding transaction: {ex.Message}");
    }
}

        static void ImportTransactions(FinancialData financialData)
{
    Console.WriteLine("Enter file path to import (CSV or JSON): ");
    var filePath = InputHelper.GetNonEmptyInput("File Path: ");

    Transaction[] importedTransactions = null;

    if (filePath.EndsWith(".csv"))
    {
        importedTransactions = DataImportExport.ParseCsvFile(filePath);
    }
    else if (filePath.EndsWith(".json"))
    {
        importedTransactions = DataImportExport.ParseJsonFile(filePath);
    }

    if (importedTransactions != null)
    {
        financialData.Transactions.AddRange(importedTransactions);
        Console.WriteLine($"{importedTransactions.Length} transactions imported successfully.");
    }
    else
    {
        Console.WriteLine("Failed to import transactions. Please check the file and try again.");
    }
}


       static void SetBudget(FinancialData financialData)
{
    try
    {
        var category = InputHelper.GetNonEmptyInput("Category: ");
        var type = InputHelper.GetValidTypeInput("Type (weekly/monthly): ", BusinessLogic.IsWeekly, BusinessLogic.IsMonthly);
        var startDate = InputHelper.GetDateInput("Start Date (yyyy-MM-dd): ");
        var amount = InputHelper.GetDecimalInput("Amount: ");

        // Automatically calculate the end date based on the budget type
        DateTime endDate = type.ToLower() == "weekly" 
            ? startDate.AddDays(7) 
            : startDate.AddMonths(1);

        financialData.Budgets.Add(new Budget
        {
            Category = category,
            Type = type,
            StartDate = startDate,
            EndDate = endDate,
            Amount = amount
        });

        Console.WriteLine($"Budget set successfully. End Date: {endDate:yyyy-MM-dd}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error setting budget: {ex.Message}");
    }
}



        static void ViewFinancialAnalytics(FinancialData financialData)
        {
            var transactionSummary = BusinessLogic.GenerateFinancialSummary(financialData.Transactions);
            UserInterface.ShowFinancialSummary(transactionSummary.ToList());

            var budgetSummary = BusinessLogic.GenerateBudgetSummary(financialData);
            UserInterface.ShowBudgetSummary(budgetSummary.ToList());

            var savingGoalsSummary = BusinessLogic.GenerateSavingGoalsSummary(financialData);
            UserInterface.ShowSavingGoalsSummary(savingGoalsSummary.ToList());
        }

        private static void ExportFinancialAnalytics(FinancialData financialData)
{
    var folderPath = InputHelper.GetNonEmptyInput("Enter the folder path where the file should be exported: ");

    try
    {
        DataImportExport.ExportFinancialAnalytics(folderPath, financialData);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error exporting data: {ex.Message}");
    }
}



static void AddGoal(FinancialData financialData)
{
    var name = InputHelper.GetNonEmptyInput("Goal Name: ");
    var target = InputHelper.GetDecimalInput("Target Amount: ");
    var startDate = InputHelper.GetDateInput("Start Date (yyyy-MM-dd): ");
    var endDate = InputHelper.GetDateInput("End Date (yyyy-MM-dd): ");

    var savingGoal = new SavingGoal
    {
        Name = name,
        Target = target,
        StartDate = startDate,
        EndDate = endDate
    };

    financialData.SavingGoals.Add(savingGoal);
    var monthlySavings = BusinessLogic.CalculateSavings(savingGoal);
    Console.WriteLine($"You need to save {monthlySavings:F2} per month to reach your goal.");
}

        

    }
}

