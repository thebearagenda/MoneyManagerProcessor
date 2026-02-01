using System;
using System.Collections.Generic;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace ProcessBudget
{
    public class MicrosoftSpreadsheetWriter : ISpreadsheetWriter
    {
        private const string s_SpreadSheetName = "Money Manager";

        private string _csvFileName = "Test.csv";
        private string _fileName;

        private Excel.Application _app;
        private Excel.Workbook _workBook;
        private Excel.Worksheet _workSheet;

        public List<DailyExpense> DailyExpenses { get; private set; }

        public MicrosoftSpreadsheetWriter(List<DailyExpense> dailyExpenses, string fileName)
        {
            try
            {
                DailyExpenses = dailyExpenses;
                _fileName = fileName;
                _app = new Excel.Application();
                PrintSpreadSheet();
            }
            catch(Exception e)
            {
                CloseApplications();
                throw e;
            }
        }

        private void PrintSpreadSheet()
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                using (StreamWriter writer = new StreamWriter(_csvFileName))
                {
                    PrintCsvHeader(writer);
                    PrintCsvData(writer);
                }
            }
            else
            {
                _workBook = _app.Workbooks.Open(_fileName);
                SetWorkSheet();
                PrintHeader();
                PrintData();
                _workBook.Save();
                _workBook.Close();
            }
        }

        private void PrintCsvHeader(StreamWriter writer)
        {
            writer.WriteLine(nameof(DailyExpense.Date) + "," +
                nameof(DailyExpense.Income) + "," +
                nameof(DailyExpense.Savings) + "," +
                nameof(DailyExpense.Expenses) + "," +
                nameof(DailyExpense.LeftOver));
        }

        private void PrintCsvData(StreamWriter writer)
        {
            for (int i = 0; i < DailyExpenses.Count; i++)
            {
                writer.WriteLine(DailyExpenses[i].Date.ToString("d/M/yyyy") + "," +
                DailyExpenses[i].Income.ToString("0.##") + "," +
                DailyExpenses[i].Savings.ToString("0.##") + "," +
                DailyExpenses[i].Expenses.ToString("0.##") + "," +
                DailyExpenses[i].LeftOver.ToString("0.##"));
            }
        }

        private void SetWorkSheet()
        {
            _workSheet = _workBook.Worksheets[s_SpreadSheetName];
            if (_workSheet != null)
            {
                _workSheet.Cells.ClearContents();
                return;
            }

            _workSheet = _workBook.Worksheets.Add(_workBook.Worksheets[_workBook.Worksheets.Count]);
            _workSheet.Name = s_SpreadSheetName;
        }

        private void PrintHeader()
        {
            _workSheet.Cells[1, 1].Value = nameof(DailyExpense.Date);
            _workSheet.Cells[1, 2].Value = nameof(DailyExpense.Income);
            _workSheet.Cells[1, 3].Value = nameof(DailyExpense.Savings);
            _workSheet.Cells[1, 4].Value = nameof(DailyExpense.Expenses);
            _workSheet.Cells[1, 5].Value = nameof(DailyExpense.LeftOver);
        }

        private void PrintData()
        {
            for (int i = 0; i < DailyExpenses.Count; i++)
            {
                _workSheet.Cells[i + 2, 1].Value = DailyExpenses[i].Date.ToString("M/d/yyyy");
                _workSheet.Cells[i + 2, 2].Value = DailyExpenses[i].Income.ToString("0.##");
                _workSheet.Cells[i + 2, 3].Value = DailyExpenses[i].Savings.ToString("0.##");
                _workSheet.Cells[i + 2, 4].Value = DailyExpenses[i].Expenses.ToString("0.##");
                _workSheet.Cells[i + 2, 5].Value = DailyExpenses[i].LeftOver.ToString("0.##");
            }
        }

        public void CloseApplications()
        {
            _workBook?.Close();
        }
    }
}
