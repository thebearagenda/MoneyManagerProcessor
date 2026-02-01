using System;

namespace ProcessBudget
{
    public class DailyExpense
    {
        private double _income;
        private double _expenses;
        private double _billsPaid;
        private double _otherIncome;
        private double _estimatedBillsLeft;

        public DateTime Date { get; private set; }
        public double Income { get; private set; }
        public double Savings { get; private set; }
        public double Expenses { get; private set; }
        public double LeftOver { get; private set; }

        public DailyExpense( DateTime date, double income, double expenses, double billsPaid, double otherIncome, double estimatedBillsLeft)
        {
            Date = date;
            _income = income;
            _expenses = expenses;
            _billsPaid = billsPaid; 
            _otherIncome = otherIncome;
            _estimatedBillsLeft = estimatedBillsLeft;

            ProcessBudgetDay();
        }

        private void ProcessBudgetDay()
        {
            Income = _income + _otherIncome;
            Savings = Income - _income * 0.2;
            Expenses = _expenses + _billsPaid + _estimatedBillsLeft;
            LeftOver = Income - _income * 0.2 - Expenses;
        }
    }
}
