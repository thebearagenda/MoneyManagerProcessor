using System;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace ProcessBudget
{
    public class MicrosoftSpreadsheetReader : ISpreadsheetReader
    {
        private const string _incomeExpense = "Income/Expense";
        private const string _date = "Date";
        private const string _category = "Category";
        private const string _amount = "Amount";


        private int _incomeExpenseColumn;
        private int _dateColumn;
        private int _categoryColumn;
        private int _amountColumn;

        private Excel.Application _app;
        private Excel.Workbook _workBook;
        private Excel.Range _cells;

        public List<BudgetEntry> Entries { get; private set; }

        public MicrosoftSpreadsheetReader(string filePath)
        {
            try
            {
                _app = new Excel.Application();
                _workBook = _app.Workbooks.Open(filePath);
                _cells = _workBook.Worksheets[1].UsedRange;
                ProcessSpreadsheet();
                _workBook.Close();
            }
            catch(Exception e)
            {
                CloseApplications();
                throw e;
            }
        }

        private void ProcessSpreadsheet()
        {
            GetEntries();
        }

        private void GetEntries()
        {
            FindColumns();
            Entries = new List<BudgetEntry>();

            for (int i = 2; i <= _cells.Rows.Count; i++)
            {
                Entries.Add(new BudgetEntry( _cells[i, _dateColumn].Text,
                    _cells[i, _incomeExpenseColumn].Text,
                    _cells[i, _categoryColumn].Text,
                    _cells[i, _amountColumn].Text));
            }
        }

        private void FindColumns()
        {
            for (int i = 1; i <= _cells.Columns.Count; i++ )
            {
                string text = _cells[1,i].Text;

                switch (text)
                {
                    case _incomeExpense:
                        _incomeExpenseColumn = i;
                        break;
                    case _date:
                        _dateColumn = i;
                        break;
                    case _category:
                        _categoryColumn = i;
                        break;
                    case _amount:
                        _amountColumn = i;
                        break;
                    default:
                        continue;
                }
            }
        }

        public void CloseApplications()
        {
            _workBook?.Close();
        }
    }
}
