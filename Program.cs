using System;
using System.IO;
using System.Windows.Forms;

namespace ProcessBudget
{
    internal class Program
    {
        public static OpenFileDialog openFileDialog;

        [STAThread]
        static void Main(string[] args)
        {
            if(!Open())
            {
                return;
            }

            ISpreadsheetReader spreadsheetReader = new MicrosoftSpreadsheetReader(openFileDialog.FileName);
            BudgetProcessor budgetProcessor = new BudgetProcessor(spreadsheetReader.Entries);
            Save(budgetProcessor);

            Console.WriteLine("Done");
            Console.WriteLine("Press a key to quit...");
            Console.ReadLine();
        }

        private static bool Open()
        {
            openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
                Title = "Import Spreadsheet",
                InitialDirectory = Properties.Settings.Default.OpenDirectory
            };

            DialogResult result = openFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog.FileName) &&  
                Properties.Settings.Default.OpenDirectory != Path.GetDirectoryName(openFileDialog.FileName))
            {
                Properties.Settings.Default.OpenDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                Properties.Settings.Default.Save();
            }

            return result == DialogResult.OK;
        }

        private static void Save( BudgetProcessor budgetProcessor )
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
                Title = "Export Spreadsheet",
                InitialDirectory = Properties.Settings.Default.OpenDirectory
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(saveFileDialog.FileName) &&
                Properties.Settings.Default.SaveDirectory != Path.GetDirectoryName(saveFileDialog.FileName))
                {
                    Properties.Settings.Default.SaveDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                    Properties.Settings.Default.Save();
                }

                if (!File.Exists(saveFileDialog.FileName))
                {
                    File.Create(saveFileDialog.FileName);
                }
            }

            new MicrosoftSpreadsheetWriter(budgetProcessor.DailyExpenses, saveFileDialog.FileName);
        }
    }
}

