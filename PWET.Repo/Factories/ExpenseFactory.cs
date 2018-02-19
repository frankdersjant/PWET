using PWET.Domain;
using PWET.DTO;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PWET.Repo.Factories
{
    public static class ExpenseFactory
    {
        public static ExpenseDTO CreateExpenseDTO(Expense expense)
        {
            return new ExpenseDTO()
            {
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description,
                ExpenseGroupId = expense.ExpenseGroupId,
                Id = expense.Id
            };
        }

        public static Expense CreateExpenseEntity(ExpenseDTO expense)
        {
            return new Expense()
            {
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description,
                ExpenseGroupId = expense.ExpenseGroupId,
                Id = expense.Id
            };
        }

        public static object CreateDataShapedObject(Expense expense, List<string> lstOfFields)
        {
            return CreateDataShapedObject(CreateExpenseDTO(expense), lstOfFields);
        }

        public static object CreateDataShapedObject(ExpenseDTO expense, List<string> lstOfFields)
        {

            if (!lstOfFields.Any())
            {
                return expense;
            }
            else
            {

                ExpandoObject objectToReturn = new ExpandoObject();
                foreach (var field in lstOfFields)
                {
                    var fieldValue = expense.GetType()
                        .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                        .GetValue(expense, null);

                    // add the field to the ExpandoObject
                    ((IDictionary<String, Object>)objectToReturn).Add(field, fieldValue);
                }
                return objectToReturn;
            }
        }

    }
}
