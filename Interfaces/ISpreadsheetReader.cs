using System.Collections.Generic;

namespace ProcessBudget
{
    public interface ISpreadsheetReader
    {
        List<BudgetEntry> Entries { get; }

        void CloseApplications();
    }
}