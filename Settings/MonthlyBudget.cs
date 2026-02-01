using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessBudget
{
    public class MonthlyBudget
    {
        public List<BudgetCategory> BudgetCategories { get; }

        public MonthlyBudget()
        {
            BudgetCategories = new List<BudgetCategory>();
            LoadBudgetCategories();
        }

        private void LoadBudgetCategories()
        {
            List<string> categories = Properties.Settings.Default.BudgetCategories.Split(';').ToList();
            categories.ForEach(category => InitializeCategory(category));
        }

        private void InitializeCategory( string category )
        {
            List<string> properties = category.Split(',').ToList();
            properties.ForEach( s => s.Trim());
            BudgetCategories.Add(new BudgetCategory(properties[0], double.Parse(properties[1]), GetBudgetType(properties[2])));
        }

        private BudgetType GetBudgetType( string s )
        {
            return (BudgetType)Enum.Parse( typeof( BudgetType ), s );
        }

        public override string ToString()
        {
            string s = string.Empty;
            BudgetCategories.ForEach(category => s += $"{category.Name}, {category.Amount}, {category.BudgetType}\n");
            return s;
        }

        public void Save()
        {
            string s = string.Empty;
            for(int i = 0; i < BudgetCategories.Count; i++)
            {
                s += $"{BudgetCategories[i].Name}, {BudgetCategories[i].Amount}, {BudgetCategories[i].BudgetType}";

                if( i != BudgetCategories.Count - 1 )
                {
                    s += ";";
                }
            }
            Properties.Settings.Default.BudgetCategories = s;
            Properties.Settings.Default.Save();
        }

        public void DeleteBudgetCategory( string name )
        {
            BudgetCategories.Remove(BudgetCategories.FirstOrDefault(c => c.Name == name));
        }

        public void AddBudgetCategory( string name, double amount, BudgetType budgetType)
        {
            BudgetCategories.Add(new BudgetCategory(name, amount, budgetType));
        }

        public void EditBudget( string name, double amount, BudgetType budgetType )
        {
            BudgetCategory budgetCategory = BudgetCategories.FirstOrDefault(bc => bc.Name == name);

            if(budgetCategory == null)
            {
                Console.WriteLine($"No budget category named '{name}'.");
                Console.WriteLine();
                return;
            }

            budgetCategory.Amount = amount;
            budgetCategory.BudgetType = budgetType;
        }
    }
}
