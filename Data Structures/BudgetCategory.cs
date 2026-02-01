namespace ProcessBudget
{
    public enum BudgetType
    {
        Ignore,
        AutoFill,
        FillIfNotPresent,
        Budgeted,
        BudgetedDecrement
    }

    public class BudgetCategory
    {
        public string Name { get; set; }
        public double Amount { get; set; }
        public BudgetType BudgetType { get; set; }

        public BudgetCategory(string name, double amount, BudgetType budgetType)
        {
            Name = name;
            Amount = amount;
            BudgetType = budgetType;
        }
    }
}
