using PWET.Domain;
using PWET.Repo.EFstatus;
using System.Linq;

namespace PWET.Repo
{
    public interface IExpenseTrackerRepository
    {
        //expensegroups
        RepositoryActionResult<ExpenseGroup> DeleteExpenseGroup(int id);
        ExpenseGroup GetExpenseGroup(int id);
        IQueryable<ExpenseGroup> GetExpenseGroups();
        IQueryable<ExpenseGroup> GetExpenseGroupsWithExpenses();
        ExpenseGroup GetExpenseGroupWithExpenses(int id);
        RepositoryActionResult<ExpenseGroup> InsertExpenseGroup(ExpenseGroup eg);
        RepositoryActionResult<ExpenseGroup> UpdateExpenseGroup(ExpenseGroup eg);

        //expensegroupstatus
       // ExpenseGroupStatus GetExpenseGroupStatus(int id);
        IQueryable<ExpenseGroupStatus> GetExpenseGroupStatusses();

        //expense
        Expense GetExpense(int id, int? expenseGroupId = null);
        RepositoryActionResult<Expense> DeleteExpense(int id);
        IQueryable<Expense> GetExpenses();
        IQueryable<Expense> GetExpenses(int expenseGroupId);
        RepositoryActionResult<Expense> InsertExpense(Expense e);
        RepositoryActionResult<Expense> UpdateExpense(Expense e);


    }
}