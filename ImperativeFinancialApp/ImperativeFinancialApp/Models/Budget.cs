namespace ImperativeFinancialApp.Models
{
    public class Budget
    {
        public string Category { get; set; }
        public string Type { get; set; } // "weekly" or "monthly"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
    }
}
