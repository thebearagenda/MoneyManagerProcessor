using System.Collections.Generic;

namespace ProcessBudget
{
    public interface IBudgetProcessor
    {
        List<DailyExpense> DailyExpenses { get; }
    }
}