using System.Text.Json;
using ImperativeFinancialApp.Models;

namespace ImperativeFinancialApp.Data
{
    public static class DataImportExport
    {
        public static Transaction[] ParseCsvFile(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                var transactions = lines.Skip(1) // Skip header
                    .Select(line =>
                    {
                        var parts = line.Split(',');
                        return new Transaction
                        {
                            Date = DateTime.Parse(parts[0]),
                            Category = parts[1],
                            Amount = decimal.Parse(parts[2]),
                            Type = parts[3]
                        };
                    }).ToArray();

                return transactions;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Transaction[] ParseJsonFile(string filePath)
{
    try
    {
        var json = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<Transaction[]>(json, options);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error importing transactions: {ex.Message}");
        return Array.Empty<Transaction>();
    }
}


       public static void ExportFinancialAnalytics(string folderPath, FinancialData data)
{
    try
    {
        EnsureDirectoryExists(folderPath);

        string filePath = Path.Combine(folderPath, "FinancialReport.json");

        var options = new JsonSerializerOptions
        {
            WriteIndented = true 
        };
        File.WriteAllText(filePath, JsonSerializer.Serialize(data, options));

        Console.WriteLine($"Financial analytics exported to: {filePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error exporting data: {ex.Message}");
    }
}

private static void EnsureDirectoryExists(string folderPath)
{
    if (!Directory.Exists(folderPath))
    {
        Directory.CreateDirectory(folderPath); 
    }
}

    }
}
