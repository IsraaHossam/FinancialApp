namespace ImperativeFinancialApp.Models
{
    public class SavingGoal
    {
        public string Name { get; set; }
        public decimal Target { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
