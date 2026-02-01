using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessBudget
{
    public class BudgetProcessor : IBudgetProcessor
    {
        private MonthlyBudget _monthlyBudget;
        private List<BudgetEntry> _budgetEntries;
        private int _days;

        public List<DailyExpense> DailyExpenses { get; private set; }

        public BudgetProcessor(List<BudgetEntry> budgetEntries)
        {
            Console.WriteLine("Up to what day of the month do you want to print up to?:");
            if (!int.TryParse(Console.ReadLine(), out int days))
            {
                days = DateTime.DaysInMonth(_budgetEntries[0].Date.Year, _budgetEntries[0].Date.Month);
            }

            EditBudget();

            _budgetEntries = budgetEntries;
            _days = days < DateTime.DaysInMonth(_budgetEntries[0].Date.Year, _budgetEntries[0].Date.Month) ?
                days : DateTime.DaysInMonth(_budgetEntries[0].Date.Year, _budgetEntries[0].Date.Month);
            ProcessBudget();
        }

        private void EditBudget()
        {
            _monthlyBudget = new MonthlyBudget();
            
            while(true)
            {
                Console.WriteLine("Do you want to edit the following budget?");
                Console.Write(_monthlyBudget.ToString());
                Console.WriteLine("0: No, 1: Edit Category, 2: Add Category, 3: Delete Category");
                Console.WriteLine();
                if (int.TryParse(Console.ReadLine(), out int result))
                {
                    switch(result)
                    {
                        case 0: 
                            break;
                        case 1:
                            EditBudgetCategory();
                            break;
                        case 2:
                            AddBudgetCategory();
                            break;
                        case 3:
                            DeleteBudgetCategory();
                            break;
                    }

                    Console.WriteLine();

                    if ( result == 0 )
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid answer, try again...");
                    Console.WriteLine();
                }
            }
        }

        private void EditBudgetCategory()
        {
            Console.WriteLine("Category's name?");
            string category = Console.ReadLine();
            Console.WriteLine("New amount?");
            double amount;
            while (!double.TryParse(Console.ReadLine(), out amount))
            {
                Console.WriteLine();
                Console.WriteLine("Invalid answer...");
                Console.WriteLine();
            }
            Console.WriteLine("Type? The following are your options:");
            foreach (BudgetType type in Enum.GetValues(typeof(BudgetType)))
            {
                Console.WriteLine(type.ToString());
            }
            BudgetType budgetType;
            Console.WriteLine();
            while (!Enum.TryParse(Console.ReadLine(), out budgetType))
            {
                Console.WriteLine("Invalid answer...");
                Console.WriteLine();
            }
            _monthlyBudget.EditBudget(category, amount, budgetType);
            _monthlyBudget.Save();
            Console.WriteLine();
        }

        private void AddBudgetCategory()
        {
            Console.WriteLine("Category's name?");
            string category = Console.ReadLine();
            Console.WriteLine("New amount?");
            double amount;
            while (!double.TryParse(Console.ReadLine(), out amount))
            {
                Console.WriteLine();
                Console.WriteLine("Invalid answer...");
                Console.WriteLine();
            }
            Console.WriteLine("Type? The following are your options:");
            foreach (BudgetType type in Enum.GetValues(typeof(BudgetType)))
            {
                Console.WriteLine(type.ToString());
            }
            BudgetType budgetType;
            while (!Enum.TryParse(Console.ReadLine(), out budgetType))
            {
                Console.WriteLine();
                Console.WriteLine("Invalid answer...");
                Console.WriteLine();
            }
            _monthlyBudget.AddBudgetCategory(category, amount, budgetType);
            _monthlyBudget.Save();
            Console.WriteLine();
        }

        private void DeleteBudgetCategory()
        {
            Console.WriteLine("Category's name?");
            string category = Console.ReadLine();
            _monthlyBudget.DeleteBudgetCategory(category);
            _monthlyBudget.Save();
            Console.WriteLine();
        }

        private void ProcessBudget()
        {
            int year = _budgetEntries[0].Date.Year;
            int month = _budgetEntries[0].Date.Month;

            DailyExpenses = new List<DailyExpense>();
            for (int i = 0; i < _days; i++)
            {
                DateTime date = new DateTime(year, month, i + 1, 23, 59, 59);
                ProcessExpensesUpToDay(date);
            }
        }

        private void ProcessExpensesUpToDay(DateTime date)
        {
            List<BudgetEntry> uncategorizedExpenses = _budgetEntries.Where(entry => entry.Date < date &&
                entry.EntryType == EntryType.Expense &&
                _monthlyBudget.BudgetCategories.All(category => category.Name != entry.Category)).ToList();
            double sumOfUncategorizedExpenses = GetSumOfEntries(uncategorizedExpenses);

            double billsPaid = 0;
            double estimatedBillsLift = 0;
            List<BudgetEntry> categorizedExpenses = _budgetEntries.Where(entry => entry.Date < date && entry.EntryType == EntryType.Expense).
                Except(uncategorizedExpenses).ToList();

            foreach (BudgetCategory budgetCategory in _monthlyBudget.BudgetCategories)
            {
                List<BudgetEntry> expensesForCategory = categorizedExpenses
                               .Where(categorizedExpense => categorizedExpense.Category == budgetCategory.Name).ToList();
                double paid = GetSumOfEntries(expensesForCategory);

                switch (budgetCategory.BudgetType)
                {
                    case BudgetType.AutoFill:
                        billsPaid += budgetCategory.Amount;
                        break;
                    case BudgetType.FillIfNotPresent:
                        if (paid > 0)
                        {
                            billsPaid += paid;
                        }
                        else
                        {
                            estimatedBillsLift += budgetCategory.Amount;
                        }
                        break;
                    case BudgetType.Budgeted:
                        billsPaid += paid;
                        estimatedBillsLift += budgetCategory.Amount - paid;
                        break;
                    case BudgetType.BudgetedDecrement:
                        billsPaid += paid;
                        double estimation = ExpensesProportionateToMonthLeft(budgetCategory, date.Day);

                        if (estimation > budgetCategory.Amount - paid)
                        {
                            estimatedBillsLift += budgetCategory.Amount - paid;
                        }
                        else
                        {
                            estimatedBillsLift += estimation;
                        }
                        break;
                    default:
                        break;
                }
            }

            double income = GetSumOfEntries(_budgetEntries.Where(entry => entry.Category == "Salary").ToList());
            double otherIncome = GetSumOfEntries(_budgetEntries.Where(entry => entry.EntryType == EntryType.Income && entry.Category != "Salary").ToList());

            Console.WriteLine($"Day {date.Day}: Income: {income:0.##}, Expenses: {sumOfUncategorizedExpenses:0.##}, " +
                $"Bills Paid: {billsPaid:0.##}, otherIncome: {otherIncome:0.##}, EBL: {estimatedBillsLift:0.##}, " +
                $"LeftOver: {income + otherIncome - sumOfUncategorizedExpenses - billsPaid - estimatedBillsLift - income * .2:0.##}");
            DailyExpenses.Add(new DailyExpense(date, income, sumOfUncategorizedExpenses, billsPaid, otherIncome, estimatedBillsLift));
        }

        private double GetSumOfEntries(List<BudgetEntry> entries)
        {
            return entries.Sum(entry => entry.Amount);
        }

        private double ExpensesProportionateToMonthLeft(BudgetCategory budgetCategory, int currentDay)
        {
            double days = (double)DateTime.DaysInMonth(_budgetEntries[0].Date.Year, _budgetEntries[0].Date.Month);
            double percent = currentDay / days;

            if (percent <= .8)
            {
                return 0.8 * budgetCategory.Amount;
            }

            if (percent <= 0.6)
            {
                return 0.6 * budgetCategory.Amount;
            }

            if (percent < 0.4)
            {
                return 0.45 * budgetCategory.Amount;
            }

            return budgetCategory.Amount;
        }
    }
}
