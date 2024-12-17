using ImperativeFinancialApp.Data;
using ImperativeFinancialApp.Models;
namespace ImperativeFinancialApp.UI
{
    public static class UserInterface
    {
        public static void ShowMainMenu()
        {
            Console.WriteLine("1. Add Transaction");
            Console.WriteLine("2. Import Transactions from File");
            Console.WriteLine("3. View Transactions");
            Console.WriteLine("4. Set Budget");
            Console.WriteLine("5. View Budgets");
            Console.WriteLine("6. Set Savings Goals");
            Console.WriteLine("7. View Financial Analytics");
            Console.WriteLine("8. Export Financial Analytics Report");
            Console.WriteLine("9. Exit");
            Console.WriteLine("Enter your choice: ");
        }

        public static void ViewTransactions(List<Transaction> transactions)
        {
            if (!transactions.Any())
            {
                Console.WriteLine("No transactions recorded.");
                return;
            }

            Console.WriteLine("Transactions:");
            foreach (var t in transactions)
            {
                Console.WriteLine($"Date: {t.Date:yyyy-MM-dd}, Category: {t.Category}, Amount: {t.Amount:F2}, Type: {t.Type}");
            }
        }

        public static void ViewBudgets(FinancialData data)
        {
            if (!data.Budgets.Any())
            {
                Console.WriteLine("No budgets set.");
                return;
            }

            Console.WriteLine("Budgets:");
            foreach (var budget in data.Budgets)
            {
                var totalSpent = BusinessLogic.CalculateSpentForCategoryInBudgetPeriod(data.Transactions, budget);
                var utilizationMessage = BusinessLogic.TrackBudgetUtilization(totalSpent, budget);
                Console.WriteLine($"Category: {budget.Category}, Type: {budget.Type}, Start: {budget.StartDate:yyyy-MM-dd}, End: {budget.EndDate:yyyy-MM-dd}, Amount: {budget.Amount:F2}, Spent: {totalSpent:F2}");
                if (!string.IsNullOrEmpty(utilizationMessage))  
                {
                    Console.WriteLine(utilizationMessage);
                }
                else
                {
                    Console.WriteLine("No utilization message.");
                }
            }
        }

        public static void ShowFinancialSummary(List<(string Category, decimal Total, DateTime StartDate, DateTime EndDate)> summary)
        {
            Console.WriteLine("Transactions Summary:");
            foreach (var (category, total, startDate, endDate) in summary)
            {
                Console.WriteLine($"Category: {category}, Total: {total:F2}, Time Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
            }
        }

        public static void ShowBudgetSummary(List<(string Category, string Type, DateTime StartDate, DateTime EndDate, decimal Amount, decimal Spent, decimal Remaining, string? Message)> budgetSummary)
        {
            Console.WriteLine("___________");
            Console.WriteLine("Budget Summary:");
            foreach (var (category, btype, startDate, endDate, amount, spent, remaining, message) in budgetSummary)
            {
                Console.WriteLine($"Category: {category}, Type: {btype}, Start: {startDate:yyyy-MM-dd}, End: {endDate:yyyy-MM-dd}, Amount: {amount:F2}, Spent: {spent:F2}, Remaining: {remaining:F2}");
                if (!string.IsNullOrEmpty(message)) 
                {
                    Console.WriteLine($"Note: {message}");
                }
            }
        }

        public static void ShowSavingGoalsSummary(List<(string Name, decimal Target, DateTime StartDate, DateTime EndDate, decimal TotalSaved, decimal Remaining, decimal MonthlyRequired)> goalsSummary)
        {
            Console.WriteLine("___________");
            Console.WriteLine("Saving Goals Summary:");
            foreach (var (name, _, startDate, endDate, _, _, monthlyRequired) in goalsSummary)
            {
                Console.WriteLine($"Goal Name: {name}, Start Date: {startDate:yyyy-MM-dd}, End Date: {endDate:yyyy-MM-dd}, Monthly Savings Required: {monthlyRequired:F2}");
            }
        }

    }
}

