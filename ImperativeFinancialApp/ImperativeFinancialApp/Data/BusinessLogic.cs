using ImperativeFinancialApp.Models;

namespace ImperativeFinancialApp.Data
{
    public static class BusinessLogic
    {
        public static bool IsIncome(string transactionType) => transactionType.ToLower() == "income";
        public static bool IsExpense(string transactionType) => transactionType.ToLower() == "expense";
        public static bool IsMonthly(string budgetType) => budgetType.ToLower() == "monthly";
        public static bool IsWeekly(string budgetType) => budgetType.ToLower() == "weekly";

        public static decimal CalculateSavings(SavingGoal goal)
        {
            int months = (int)((goal.EndDate - goal.StartDate).TotalDays / 30.0);
            return months > 0 ? goal.Target / months : 0M;
        }

        public static string TrackBudgetUtilization(decimal spent, Budget budget)
        {
            var utilization = spent / budget.Amount * 100M;
            if (utilization > 90M) 
                return $"Warning: Budget utilization is at {utilization:F2}%.";
            if (utilization > 50M)
                return $"Take care: You have spent {utilization:F2}% of your budget.";
            return $"Budget utilization is around {utilization:F2}%.";
        }

        public static IEnumerable<(string Category, decimal Total, DateTime MinDate, DateTime MaxDate)> GenerateFinancialSummary(IEnumerable<Transaction> transactions)
        {
            return transactions.GroupBy(t => t.Category).Select(g => 
            {
                var total = g.Sum(t => t.Amount);
                var minDate = g.Min(t => t.Date);
                var maxDate = g.Max(t => t.Date);
                return (g.Key, total, minDate, maxDate);
            }).ToList();
        }

        public static decimal CalculateSpentForCategoryInBudgetPeriod(IEnumerable<Transaction> transactions, Budget budget)
        {
            return transactions
                .Where(t => t.Category == budget.Category && IsExpense(t.Type) && t.Date >= budget.StartDate && t.Date <= budget.EndDate)
                .Sum(t => t.Amount);
        }

        public static IEnumerable<(string Category, string Type, DateTime StartDate, DateTime EndDate, decimal Amount, decimal Spent, decimal Remaining, string UtilizationMessage)> GenerateBudgetSummary(FinancialData data)
        {
            return data.Budgets.Select(budget =>
            {
                var spent = CalculateSpentForCategoryInBudgetPeriod(data.Transactions, budget);
                var remaining = budget.Amount - spent;
                var utilizationMessage = TrackBudgetUtilization(spent, budget);
                return (budget.Category, budget.Type, budget.StartDate, budget.EndDate, budget.Amount, spent, remaining, utilizationMessage);
            }).ToList();
        }

        public static IEnumerable<(string Name, decimal Target, DateTime StartDate, DateTime EndDate, decimal TotalSaved, decimal Remaining, decimal MonthlySavingsRequired)> GenerateSavingGoalsSummary(FinancialData data)
        {
            return data.SavingGoals.Select(goal =>
            {
                int months = (int)((goal.EndDate - goal.StartDate).TotalDays / 30.0);
                var totalSaved = data.Transactions
                    .Where(t => t.Category == goal.Name && IsIncome(t.Type))
                    .Sum(t => t.Amount);
                var remaining = goal.Target - totalSaved;
                var monthlySavingsRequired = CalculateSavings(goal);
                return (goal.Name, goal.Target, goal.StartDate, goal.EndDate, totalSaved, remaining, monthlySavingsRequired);
            }).ToList();
        }
    }
}
