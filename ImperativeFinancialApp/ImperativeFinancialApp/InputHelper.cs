public static class InputHelper
{
    public static string GetNonEmptyInput(string prompt)
    {
        string input;
        do
        {
            Console.Write(prompt);
            input = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty. Please enter a valid value.");
            }
        } while (string.IsNullOrWhiteSpace(input));

        return input;
    }

    public static DateTime GetDateInput(string prompt)
    {
        DateTime date;
        string dateInput;
        do
        {
            dateInput = GetNonEmptyInput(prompt);
            if (!DateTime.TryParse(dateInput, out date))
            {
                Console.WriteLine("Invalid date format. Please enter a valid date (yyyy-MM-dd).");
            }
        } while (!DateTime.TryParse(dateInput, out date));

        return date;
    }

    public static decimal GetDecimalInput(string prompt)
    {
        decimal value;
        string decimalInput;
        do
        {
            decimalInput = GetNonEmptyInput(prompt);
            if (!decimal.TryParse(decimalInput, out value))
            {
                Console.WriteLine("Invalid decimal value. Please enter a valid number.");
            }
        } while (!decimal.TryParse(decimalInput, out value));

        return value;
    }

    public static string GetValidTypeInput(string prompt, Func<string, bool> isValidIncomeType, Func<string, bool> isValidExpenseType)
    {
        string input;
        do
        {
            input = GetNonEmptyInput(prompt).ToLower();
            if (!isValidIncomeType(input) && !isValidExpenseType(input))
            {
                Console.WriteLine("Invalid input. Must be 'income' or 'expense'.");
            }
        } while (!isValidIncomeType(input) && !isValidExpenseType(input));

        return input;
    }
}
