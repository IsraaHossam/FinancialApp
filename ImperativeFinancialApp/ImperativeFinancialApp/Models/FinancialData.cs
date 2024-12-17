namespace ImperativeFinancialApp.Models
{
    public class FinancialData
    {
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public List<Budget> Budgets { get; set; } = new List<Budget>();
        public List<SavingGoal> SavingGoals { get; set; } = new List<SavingGoal>();
    }
}
