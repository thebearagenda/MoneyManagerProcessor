using SpreadsheetLight;
using System.Collections.Generic;
using System.IO;

namespace ProcessBudget
{
    public class SpreadsheetWriter : ISpreadsheetWriter
    {
        private const string s_SpreadSheetName = "Money Manager";

        private string _csvFileName = "Test.csv";
        private string _fileName;

        public List<DailyExpense> DailyExpenses { get; private set; }

        public SpreadsheetWriter(List<DailyExpense> dailyExpenses, string fileName)
        {
            DailyExpenses = dailyExpenses;
            _fileName = fileName;
            PrintSpreadSheet();
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
                SLDocument spreadSheet = new SLDocument(_fileName);
                SetWorkSheet(spreadSheet);
                PrintHeader(spreadSheet);
                PrintData(spreadSheet);
                spreadSheet.Save();
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

        private void SetWorkSheet(SLDocument spreadSheet)
        {
            if (spreadSheet.SelectWorksheet(s_SpreadSheetName))
            {
                spreadSheet.ClearPrintArea();
                return;
            }

            spreadSheet.AddWorksheet(s_SpreadSheetName);
            spreadSheet.SelectWorksheet(s_SpreadSheetName);
        }

        private void PrintHeader(SLDocument spreadSheet)
        {
            spreadSheet.SetCellValue(1, 1, nameof(DailyExpense.Date));
            spreadSheet.SetCellValue(1, 2, nameof(DailyExpense.Income));
            spreadSheet.SetCellValue(1, 3, nameof(DailyExpense.Savings));
            spreadSheet.SetCellValue(1, 4, nameof(DailyExpense.Expenses));
            spreadSheet.SetCellValue(1, 5, nameof(DailyExpense.LeftOver));
        }

        private void PrintData(SLDocument spreadSheet)
        {
            for (int i = 0; i < DailyExpenses.Count; i++)
            {
                spreadSheet.SetCellValue(i + 2, 1, DailyExpenses[i].Date.ToString("d/M/yyyy"));
                spreadSheet.SetCellValue(i + 2, 2, DailyExpenses[i].Income.ToString("0.##"));
                spreadSheet.SetCellValue(i + 2, 3, DailyExpenses[i].Savings.ToString("0.##"));
                spreadSheet.SetCellValue(i + 2, 4, DailyExpenses[i].Expenses.ToString("0.##"));
                spreadSheet.SetCellValue(i + 2, 5, DailyExpenses[i].LeftOver.ToString("0.##"));
            }
        }

        public void CloseApplications()
        {
            // do nothing
        }
    }
}
