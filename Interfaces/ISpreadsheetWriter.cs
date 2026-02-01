using System.Collections.Generic;

namespace ProcessBudget
{
    public interface ISpreadsheetWriter
    {
        List<DailyExpense> DailyExpenses { get; }

        void CloseApplications();
    }
}