using System;
using System.Globalization;

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
            Date = ParseStringForDateTime(date);
            EntryType = entryType == EntryType.Income.ToString() ? EntryType.Income : EntryType.Expense;
            Category = category;
            Amount = double.Parse(amount);
        }

        private DateTime ParseStringForDateTime(string s)
        {
            DateTime date;

            try
            {
                date = DateTime.Parse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            }
            catch (FormatException)
            {
                date = DateTime.Parse(s, new CultureInfo("en-GB")); // My phone uses GB format.
            }

            return date;
        }
    }
}
