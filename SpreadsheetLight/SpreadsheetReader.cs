using System.Collections.Generic;
using SpreadsheetLight;

namespace ProcessBudget
{
    public class SpreadsheetReader : ISpreadsheetReader
    {
        private const string _incomeExpense = "Income/Expense";
        private const string _date = "Date";
        private const string _category = "Category";
        private const string _amount = "Amount";

        private SLDocument _spreadSheet;

        private int _incomeExpenseColumn;
        private int _dateColumn;
        private int _categoryColumn;
        private int _amountColumn;

        public List<BudgetEntry> Entries { get; private set; }

        public SpreadsheetReader(string filePath)
        {
            _spreadSheet = new SLDocument(filePath);
            ProcessSpreadSheet();
            _spreadSheet.CloseWithoutSaving();
        }

        private void ProcessSpreadSheet()
        {
            GetEntries();
        }

        private void GetEntries()
        {
            FindColumns();
            Entries = new List<BudgetEntry>();

            for (int i = 1; i < _spreadSheet.GetCells().Count; i++)
            {
                Entries.Add(new BudgetEntry(_spreadSheet.GetCellValueAsString(i + 1, _dateColumn),
                    _spreadSheet.GetCellValueAsString(i + 1, _incomeExpenseColumn),
                    _spreadSheet.GetCellValueAsString(i + 1, _categoryColumn),
                    _spreadSheet.GetCellValueAsString(i + 1, _amountColumn)));
            }
        }

        private void FindColumns()
        {
            for (int i = 0; i < _spreadSheet.GetCells()[1].Count; i++)
            {
                string text = _spreadSheet.GetCellValueAsString(1, i + 1);

                switch (text)
                {
                    case _incomeExpense:
                        _incomeExpenseColumn = i + 1;
                        break;
                    case _date:
                        _dateColumn = i + 1;
                        break;
                    case _category:
                        _categoryColumn = i + 1;
                        break;
                    case _amount:
                        _amountColumn = i + 1;
                        break;
                    default:
                        continue;
                }
            }
        }

        public void CloseApplications()
        {
            // do nothing
        }
    }
}
