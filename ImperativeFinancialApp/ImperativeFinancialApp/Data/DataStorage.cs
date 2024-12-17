using System.Text.Json;
using ImperativeFinancialApp.Models;

namespace ImperativeFinancialApp.Data
{
    public static class DataStorage
    {
        public static JsonSerializerOptions GetJsonOptions() =>
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

        public static FinancialData LoadData(string filePath, JsonSerializerOptions options)
        {
            if (File.Exists(filePath))
            {
                var jsonData = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<FinancialData>(jsonData, options) ?? new FinancialData();
            }
            return new FinancialData(); 
        }

       
        public static FinancialData SaveData(FinancialData newData, string directoryPath, string fileName, JsonSerializerOptions options)
        {
            string filePath = Path.Combine(directoryPath, fileName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath); 
            }

            var existingData = LoadData(filePath, options);
            MergeData(existingData, newData);
            File.WriteAllText(filePath, JsonSerializer.Serialize(existingData, options));

            return existingData;
        }


        private static void MergeData(FinancialData existingData, FinancialData newData)
        {
            
            existingData.Transactions.AddRange(
                newData.Transactions.FindAll(t => 
                    !existingData.Transactions.Exists(e => 
                        e.Date == t.Date && e.Category == t.Category && e.Type == t.Type)
                )
            );

           
            existingData.Budgets.AddRange(
                newData.Budgets.FindAll(b => 
                    !existingData.Budgets.Exists(e => 
                        e.Category == b.Category && e.Type == b.Type)
                )
            );

           
            existingData.SavingGoals.AddRange(
                newData.SavingGoals.FindAll(g => 
                    !existingData.SavingGoals.Exists(e => 
                        e.Target == g.Target && e.StartDate == g.StartDate && e.EndDate == g.EndDate)
                )
            );
        }
    }
}
