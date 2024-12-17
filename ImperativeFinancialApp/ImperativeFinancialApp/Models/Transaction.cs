namespace ImperativeFinancialApp.Models
{
    public class Transaction
    {
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // "income" or "expense"
    }
}
