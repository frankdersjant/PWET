using PWET.Domain;
using PWET.Repo.Context;
using PWET.Repo.EFstatus;
using System;
using System.Data.Entity;
using System.Linq;

namespace PWET.Repo
{
    public class ExpenseTrackerRepository : IExpenseTrackerRepository
    {
        private PWETContext _ctx;

        public ExpenseTrackerRepository(PWETContext context)
        {
            _ctx = context;
            _ctx.Configuration.LazyLoadingEnabled = false;
        }

        public RepositoryActionResult<Expense> DeleteExpense(int id)
        {
            try
            {
                Expense exp = _ctx.Expense.Where(i => i.Id == id).FirstOrDefault();
                if (exp != null)
                {
                    _ctx.Expense.Remove(exp);
                    _ctx.SaveChanges();
                    return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.Deleted);
                }
                return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.NotFound);
            }
            catch (Exception ex)
            {
               return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public  RepositoryActionResult<ExpenseGroup> DeleteExpenseGroup(int id)
        {
            try
            {

                var eg = _ctx.ExpenseGroups.Where(e => e.Id == id).FirstOrDefault();
                if (eg != null)
                {
                    _ctx.ExpenseGroups.Remove(eg);
                    _ctx.SaveChanges();
                    return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.Deleted);
                }
                return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.NotFound);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public Expense GetExpense(int id, int? expenseGroupId = default(int?))
        {
            return _ctx.Expense.FirstOrDefault(e => e.Id == id && (expenseGroupId == null || expenseGroupId == e.ExpenseGroupId));
        }

        public ExpenseGroup GetExpenseGroup(int id)
        {
            return _ctx.ExpenseGroups.FirstOrDefault(eg => eg.Id == id);
        }

        public IQueryable<ExpenseGroup> GetExpenseGroups()
        {
            return _ctx.ExpenseGroups;
        }

        //public ExpenseGroupStatus GetExpenseGroupStatus(int id)
        //{
        //    throw new NotImplementedException();
        //}

        public IQueryable<ExpenseGroupStatus> GetExpenseGroupStatusses()
        {
            return _ctx.ExpenseGroupStatusses;
        }

        public IQueryable<ExpenseGroup> GetExpenseGroupsWithExpenses()
        {
            return _ctx.ExpenseGroups.Include(p => p.Expenses); 
        }

        public ExpenseGroup GetExpenseGroupWithExpenses(int id)
        {
            return _ctx.ExpenseGroups.Where(i => i.Id == id).Include(p => p.Expenses).First();
        }

        public IQueryable<Expense> GetExpenses()
        {
            return _ctx.Expense;
        }

        public IQueryable<Expense> GetExpenses(int expenseGroupId)
        {
            return _ctx.Expense.Include(i => i.ExpenseGroup).Where(p => p.ExpenseGroupId == expenseGroupId);
        }

        public RepositoryActionResult<Expense> InsertExpense(Expense e)
        {
            try
            {
                _ctx.Expense.Add(e);
                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.Created);
                }
                else
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.NothingModified, null);
                }

            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public  RepositoryActionResult<ExpenseGroup> InsertExpenseGroup(ExpenseGroup eg)
        {
            try
            {
                _ctx.ExpenseGroups.Add(eg);
                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.Created);
                }
                else
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.NothingModified, null);
                }
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public RepositoryActionResult<Expense> UpdateExpense(Expense e)
        {
            try
            {
                var existingExpense = _ctx.Expense.FirstOrDefault(exp => exp.Id == e.Id);
                if (existingExpense == null)
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.NotFound);
                }
                _ctx.Entry(existingExpense).State = EntityState.Detached;
                _ctx.Expense.Attach(e);
                _ctx.Entry(e).State = EntityState.Modified;

                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.Updated);
                }
                else
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.NothingModified, null);
                }
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public RepositoryActionResult<ExpenseGroup> UpdateExpenseGroup(ExpenseGroup eg)
        {
            try
            {
                var existingEG = _ctx.ExpenseGroups.FirstOrDefault(exg => exg.Id == eg.Id);

                if (existingEG == null)
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.NotFound);
                }

                _ctx.Entry(existingEG).State = EntityState.Detached;
                _ctx.ExpenseGroups.Attach(eg);
                _ctx.Entry(eg).State = EntityState.Modified;
                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.Updated);
                }
                else
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.NothingModified, null);
                }

            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.Error, ex);
            }
        }
    }
}
