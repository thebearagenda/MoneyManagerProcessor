using System;

namespace ProcessBudget
{
    public enum EntryType
    {
        Income,
        Expense
    }

    public class BudgetEntry
    {
        public DateTime Date { get; private set; }
        public EntryType EntryType { get; private set; }
        public string Category { get; private set; }
        public double Amount { get; private set; }

        public BudgetEntry(string date, string entryType, string category, string amount)
        {
            Date = DateTime.Parse(date);
            EntryType = entryType == EntryType.Income.ToString() ? EntryType.Income : EntryType.Expense;
            Category = category;
            Amount = double.Parse(amount);
        }
    }
}
