open System
open System.IO
open System.Text.Json
open System.Text.Json.Serialization

// Models
type Transaction = {
    Date: DateTime
    Category: string
    Amount: decimal
    Type: string // income | expense
}

type Budget = {
    Category: string
    Type: string // weekly | monthly
    StartDate: DateTime
    EndDate: DateTime
    Amount: decimal
}

type SavingGoal = {
    Name: string 
    Target: decimal
    StartDate: DateTime
    EndDate: DateTime
}

type FinancialData = {
    Transactions: Transaction list
    Budgets: Budget list
    SavingGoals: SavingGoal list
}




// Helper functions for checking types (with case-insensitivity)
let isIncome (transactionType: string) = transactionType.ToLower() = "income"
let isExpense (transactionType: string) = transactionType.ToLower() = "expense"
let isMonthly (budgetType: string) = budgetType.ToLower() = "monthly"
let isWeekly (budgetType: string) = budgetType.ToLower() = "weekly"

// Helper function to validate date input
let rec readDate prompt =
    printf "%s" prompt
    match DateTime.TryParse(Console.ReadLine()) with
    | (true, date) -> date // Return the valid date
    | (false, _) -> 
        printfn "Invalid date format. Please enter a valid date (yyyy-MM-dd)."
        readDate prompt // Prompt the user to enter a valid date again




// Custom implementation of map
let rec myMap f lst = 
    match lst with
    | [] -> []
    | x :: xs -> (f x) :: myMap f xs

// Custom implementation of filter
let rec myFilter predicate lst =
    match lst with
    | [] -> []
    | x :: xs when predicate x -> x :: myFilter predicate xs
    | _ :: xs -> myFilter predicate xs

// Custom implementation of groupBy
let myGroupBy keySelector lst =
    let rec groupHelper remaining result =
        match remaining with
        | [] -> result
        | x :: xs ->
            let key = keySelector x
            let (groupWithKey, otherGroups) = List.partition (fun (k, _) -> k = key) result
            match groupWithKey with
            | [] ->  
                groupHelper xs ((key, [x]) :: result)
            | (key', groupItems) :: _ ->  
                let updatedGroup = (key, x :: groupItems)  
                groupHelper xs ((updatedGroup) :: (List.filter (fun (k, _) -> k <> key) result))  
    groupHelper lst []

// Custom implementation of SumBy
let rec mySumBy f lst =
    match lst with
    | [] -> 0.0M  
    | x :: xs -> f x + mySumBy f xs

// Custom implementation of MinBy
let myMinBy f lst =
    match lst with
    | [] -> failwith "List cannot be empty"
    | x :: xs -> 
        let rec findMin minSoFar remaining =
            match remaining with
            | [] -> minSoFar
            | y :: ys -> 
                if f y < f minSoFar then findMin y ys
                else findMin minSoFar ys
        findMin x xs

// Custom implementation of MaxBy
let myMaxBy f lst =
    match lst with
    | [] -> failwith "List cannot be empty"
    | x :: xs -> 
        let rec findMax maxSoFar remaining =
            match remaining with
            | [] -> maxSoFar
            | y :: ys -> 
                if f y > f maxSoFar then findMax y ys
                else findMax maxSoFar ys
        findMax x xs




// Business Logic
module BusinessLogic =
    let calculateSavings (goal: SavingGoal) =
        let months = int ((goal.EndDate - goal.StartDate).TotalDays / 30.0)
        if months > 0 then
            goal.Target / decimal months
        else
            0M

    let trackBudgetUtilization (spent: decimal) (budget: Budget) =
        let utilization = spent / budget.Amount * 100M
        if utilization > 90M then 
            Some (sprintf "Warning: Budget utilization is at %.2f%%." utilization)
        elif utilization > 50M then
            Some (sprintf "Take care: You have spent %.2f%% of your budget." utilization)
        else 
            Some (sprintf "Budget utilization is around %.2f%%." utilization)


    let generateTransactionSummary (transactions: Transaction list) =
      transactions
    |> myGroupBy (fun (t: Transaction) -> t.Category) 
    |> myMap (fun (category, trans) -> 
        let total = trans |> mySumBy (fun t -> t.Amount)
        let minDate = trans |> myMinBy (fun t -> t.Date) |> fun t -> t.Date  // Extract the Date field
        let maxDate = trans |> myMaxBy (fun t -> t.Date) |> fun t -> t.Date  // Extract the Date field
        category, total, minDate, maxDate)



    let calculateSpentForCategoryInBudgetPeriod (transactions: Transaction list) (budget: Budget) =
        transactions
        |> myFilter (fun t -> 
            t.Category = budget.Category &&
            isExpense t.Type &&
            t.Date >= budget.StartDate &&
            t.Date <= budget.EndDate)
        |> mySumBy (fun t -> t.Amount)
    

    let addSavingGoal () =
        printf "Goal Name: "
        let name = Console.ReadLine()
        printf "Target Amount: "
        let target = decimal (Console.ReadLine())
        printf "Start Date (yyyy-MM-dd): "
        let startDate = DateTime.Parse(Console.ReadLine())
        printf "End Date (yyyy-MM-dd): "
        let endDate = DateTime.Parse(Console.ReadLine())
        let savingGoal = { Name = name; Target = target; StartDate = startDate; EndDate = endDate }
        let monthlySavings = calculateSavings savingGoal
        printfn "You need to save %.2f per month to reach your goal." monthlySavings
        savingGoal





// User Interface
module UserInterface =
    let showMainMenu () =
        printfn "1. Add Transaction"
        printfn "2. Import Transactions from File"
        printfn "3. View Transactions"
        printfn "4. Set Budget"
        printfn "5. View Budgets"
        printfn "6. Set Savings Goals"
        printfn "7. View Financial Analytics"
        printfn "8. export financial analytics report"
        printfn "9. Exit"
        printfn "Enter your choice: "

    let viewTransactions (transactions: Transaction list) =
        if List.isEmpty transactions then
            printfn "No transactions recorded."
        else
            printfn "Transactions:"
            transactions 
            |> List.iter (fun t -> 
                printfn "Date: %s, Category: %s, Amount: %.2f, Type: %s"
                    (t.Date.ToString("yyyy-MM-dd"))
                    t.Category
                    t.Amount
                    t.Type)

    let viewBudgets (data: FinancialData) =
        if List.isEmpty data.Budgets then
            printfn "No budgets set."
        else
            printfn "Budgets:"
            data.Budgets
            |> List.iter (fun budget -> 
                let totalSpent = BusinessLogic.calculateSpentForCategoryInBudgetPeriod data.Transactions budget
                let utilizationMessage = BusinessLogic.trackBudgetUtilization totalSpent budget
                printfn "Category: %s, Type: %s, Start: %s, End: %s, Amount: %.2f, Spent: %.2f"
                    budget.Category
                    budget.Type
                    (budget.StartDate.ToString("yyyy-MM-dd"))
                    (budget.EndDate.ToString("yyyy-MM-dd"))
                    budget.Amount
                    totalSpent
                match utilizationMessage with
                | Some message -> printfn "%s" message
                | None -> printfn "No utilization message.")

    let showFinancialSummary (summary: (string * decimal * DateTime * DateTime) list) =
        printfn "Transactions Summary:"
        summary 
        |> List.iter (fun (category, total, startDate, endDate) -> 
            printfn "Category: %s, Total: %.2f, Time Period: %s to %s" 
                category 
                total 
                (startDate.ToString("yyyy-MM-dd")) 
                (endDate.ToString("yyyy-MM-dd")))




// Utility to configure JSON options
let getJsonOptions () =
    let options = JsonSerializerOptions(PropertyNameCaseInsensitive = true)
    options.Converters.Add(JsonStringEnumConverter()) // Enables enum serialization
    options

// Data Storage
module DataStorage =
    let options = getJsonOptions()

    // Helper functions to check if data exists
    let transactionExists (transaction: Transaction) (existingTransactions: Transaction list) =
        existingTransactions |> List.exists (fun t -> t.Date = transaction.Date && t.Category = transaction.Category && t.Type = t.Type)

    let budgetExists (budget: Budget) (existingBudgets: Budget list) =
        existingBudgets |> List.exists (fun b -> b.Category = budget.Category && b.Type = b.Type)

    let savingGoalExists (goal: SavingGoal) (existingGoals: SavingGoal list) =
        existingGoals |> List.exists (fun g -> g.Target = goal.Target && g.StartDate = goal.StartDate && g.EndDate = g.EndDate)

    // Purely functional saveData: Returns new financial data after applying the updates
    let saveData (filePath: string) (newData: FinancialData) =
        let existingData =
            if File.Exists(filePath) then
                let json = File.ReadAllText(filePath)
                JsonSerializer.Deserialize<FinancialData>(json, options)
            else
                { Transactions = []; Budgets = []; SavingGoals = [] }

        
        let removeDuplicates newItems existingItems existsFunc = // higher order function 'pass fun as argument'
            newItems |> List.filter (fun item -> not (existsFunc item existingItems))

        
        let uniqueTransactions = removeDuplicates newData.Transactions existingData.Transactions transactionExists
        let uniqueBudgets = removeDuplicates newData.Budgets existingData.Budgets budgetExists
        let uniqueSavingGoals = removeDuplicates newData.SavingGoals existingData.SavingGoals savingGoalExists

        
        let mergedData = {
            Transactions = uniqueTransactions @ existingData.Transactions
            Budgets = uniqueBudgets @ existingData.Budgets
            SavingGoals = uniqueSavingGoals @ existingData.SavingGoals
        }

        
        let json = JsonSerializer.Serialize(mergedData, options)
        File.WriteAllText(filePath, json)

        mergedData 

    let loadData (filePath: string) =
        if File.Exists(filePath) then
            let json = File.ReadAllText(filePath)
            JsonSerializer.Deserialize<FinancialData>(json, options)
        else
            { Transactions = []; Budgets = []; SavingGoals = [] }

// CSV Parsing Function
let parseCsvFile (filePath: string) =
    try
        let lines = File.ReadAllLines(filePath)
        let transactions =
            lines
            |> Array.skip 1 // Skip header
            |> Array.map (fun line -> 
                let parts = line.Split(',')
                let date = DateTime.Parse(parts.[0])
                let category = parts.[1]
                let amount = decimal parts.[2]
                let ttype = parts.[3]
                { Date = date; Category = category; Amount = amount; Type = ttype })
        Some transactions
    with
    | :? System.Exception -> None

// JSON Parsing Function
let parseJsonFile (filePath: string) =
    try
        let json = File.ReadAllText(filePath)
        let transactions = JsonSerializer.Deserialize<Transaction[]>(json, DataStorage.options)
        Some transactions
    with
    | :? System.Exception -> None 




module DataExport =

    // Helper function to ensure the directory exists
    let ensureDirectoryExists (folderPath: string) =
        let directoryPath = Path.GetDirectoryName(folderPath) 
        if not (Directory.Exists(directoryPath)) then
            ignore (Directory.CreateDirectory(directoryPath)) 

    
    let exportFinancialAnalytics (fileName: string) (data: FinancialData) =
        printf "Enter the folder path to save the file (e.g., './exports'): "
        let folderPath = Console.ReadLine() // Let the user specify the folder path
        let fullPath = Path.Combine(folderPath, fileName) // Combine the folder path with the file name
        ensureDirectoryExists fullPath // Ensure the directory exists before writing the file
        let json = JsonSerializer.Serialize(data, DataStorage.options)
        File.WriteAllText(fullPath, json)

        printfn "Financial analytics exported to: %s" fullPath


// Main Program
[<EntryPoint>]
let main _ =
    let dataPath = "financialData.json"
    let mutable data = DataStorage.loadData dataPath

    let rec mainMenu () =
        UserInterface.showMainMenu ()
        match Console.ReadLine() with
        | "1" -> 
            let date = readDate "Date (yyyy-MM-dd): " 
            printf "Category: "
            let category = Console.ReadLine()
            printf "Amount: "
            let amount = decimal (Console.ReadLine())
            printf "Type (Income/Expense): "
            let ttype = Console.ReadLine()
            let transaction = { Date = date; Category = category; Amount = amount; Type = ttype }
            data <- { data with Transactions = transaction :: data.Transactions }
            printfn "Transaction added."
            mainMenu()
        | "2" -> 
            printf "Enter file path for CSV or JSON: "
            let filePath = Console.ReadLine()
            match parseCsvFile filePath with
            | Some transactions -> 
                let transactionList = List.ofArray transactions // Convert array to list
                data <- { data with Transactions = transactionList @ data.Transactions }
                printfn "Transactions imported from CSV."
            | None -> 
                match parseJsonFile filePath with
                | Some transactions -> 
                    let transactionList = List.ofArray transactions // Convert array to list
                    data <- { data with Transactions = transactionList @ data.Transactions }
                    printfn "Transactions imported from JSON."
                | None -> 
                    printfn "Something went wrong. Check the path or file content."
            mainMenu()
        | "3" -> 
            UserInterface.viewTransactions data.Transactions
            mainMenu()
        | "4" -> 
            printf "Category: "
            let category = Console.ReadLine()
            printf "Budget Type (Monthly/Weekly): "
            let budgetType = Console.ReadLine().ToLower() 
            let startDate = readDate "Start Date (yyyy-MM-dd): " 
            printf "Budget Amount: "
            let amount = decimal (Console.ReadLine())
            
            let endDate =
                if isMonthly budgetType then
                    startDate.AddMonths(1).AddDays(-1.0) 
                else if isWeekly budgetType then
                    startDate.AddDays(6.0) 
                else
                    failwith "Invalid budget type."

            let budget = { Category = category; Type = budgetType; StartDate = startDate; EndDate = endDate; Amount = amount }
            data <- { data with Budgets = budget :: data.Budgets }
            printfn "Budget added."
            mainMenu()
        | "5" -> 
            UserInterface.viewBudgets data
            mainMenu()
        | "6" -> 
            let savingGoal = BusinessLogic.addSavingGoal ()
            data <- { data with SavingGoals = savingGoal :: data.SavingGoals }
            printfn "Saving goal added."
            mainMenu()
        | "7" -> 
            let transactionSummary = BusinessLogic.generateTransactionSummary data.Transactions
            UserInterface.showFinancialSummary transactionSummary
            mainMenu()
        | "8" -> 
            DataExport.exportFinancialAnalytics "financial_analytics.json" data
            mainMenu()
            
        | "9" -> 
            DataStorage.saveData dataPath data |> ignore
            printfn "Data saved. Exiting..."
        | _ -> 
            printfn "Invalid choice. Try again."
            mainMenu()

    mainMenu()
    0 
