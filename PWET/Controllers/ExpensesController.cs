using Marvin.JsonPatch;
using PWET.Domain;
using PWET.DTO;
using PWET.Helpers;
using PWET.Repo;
using PWET.Repo.Context;
using PWET.Repo.EFstatus;
using PWET.Repo.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;

namespace PWET.Controllers
{
    public class ExpensesController : ApiController
    {
        private IExpenseTrackerRepository _repo { get; set; }
        const int maxPageSize = 5;

        public ExpensesController()
        {
            _repo = new ExpenseTrackerRepository(new PWETContext());
        }

        public ExpensesController(IExpenseTrackerRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Route("api/expensegroup/{expenseGroupId}/expenses", Name = "ExpensesForGroup")]
        public IHttpActionResult Get(int expenseGroupId, string fields = null, string sort = "date", int page = 1, int pageSize = maxPageSize)
        {
            try
            {
                List<string> lstOfFields = new List<string>();

                if (fields != null)
                {
                    lstOfFields = fields.ToLower().Split(',').ToList();
                }

                var expenses = _repo.GetExpenses(expenseGroupId);

                if (expenses == null)
                {
                    return NotFound();
                }

                if (pageSize > maxPageSize)
                {
                    pageSize = maxPageSize;
                }

                var totalCount = expenses.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var urlHelper = new UrlHelper(Request);

                var prevLink = page > 1 ? urlHelper.Link("ExpensesForGroup",
                    new
                    {
                        page = page - 1,
                        pageSize = pageSize,
                        expenseGroupId = expenseGroupId,
                        fields = fields,
                        sort = sort
                    }) : "";
                var nextLink = page < totalPages ? urlHelper.Link("ExpensesForGroup",
                    new
                    {
                        page = page + 1,
                        pageSize = pageSize,
                        expenseGroupId = expenseGroupId,
                        fields = fields,
                        sort = sort
                    }) : "";


                var paginationHeader = new
                {
                    currentPage = page,
                    pageSize = pageSize,
                    totalCount = totalCount,
                    totalPages = totalPages,
                    previousPageLink = prevLink,
                    nextPageLink = nextLink
                };

                HttpContext.Current.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

                var expensesResult = expenses
                    .ApplySort(sort)
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize)
                    .ToList()
                    .Select(exp => ExpenseFactory.CreateDataShapedObject(exp, lstOfFields));

                return Ok(expensesResult);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        public IHttpActionResult Get(int id, int? expenseGroupId = null,  string fields = null)
        {
            try
            {
                List<string> lstOfFields = new List<string>();

                if (fields != null)
                {
                    lstOfFields = fields.ToLower().Split(',').ToList();
                }

                Expense expense = null;

                if (expenseGroupId == null)
                {
                    expense = _repo.GetExpense(id);
                }
                else
                {
                    var expensesForGroup = _repo.GetExpenses((int)expenseGroupId);

                    // if the group doesn't exist, we shouldn't try to get the expenses
                    if (expensesForGroup != null)
                    {
                        expense = expensesForGroup.FirstOrDefault(eg => eg.Id == id);
                    }
                }

                if (expense != null)
                {
                    var returnValue = ExpenseFactory.CreateDataShapedObject(expense, lstOfFields);
                    return Ok(returnValue);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


        public IHttpActionResult Post([FromBody] ExpenseDTO expense)
        {
            try
            {
                if (expense == null)
                {
                    return BadRequest();
                }

                var exp = ExpenseFactory.CreateExpenseEntity(expense);
                var result = _repo.InsertExpense(exp);
                if (result.Status == RepositoryActionStatus.Created)
                {
                    var newExp = ExpenseFactory.CreateExpenseDTO(result.Entity);
                    return Created<ExpenseDTO>(Request.RequestUri + "/" + newExp.Id.ToString(), newExp);
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        public IHttpActionResult Delete(int id)
        {
            try
            {
                var result = _repo.DeleteExpense(id);
                if (result.Status == RepositoryActionStatus.Deleted)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody]JsonPatchDocument<ExpenseDTO> expensePatchDocument)
        {
            try
            {
                if (expensePatchDocument == null)
                {
                    return BadRequest();
                }

                var expense = _repo.GetExpense(id);
                if (expense == null)
                {
                    return NotFound();
                }

                var exp = ExpenseFactory.CreateExpenseDTO(expense);
                expensePatchDocument.ApplyTo(exp);
                var result = _repo.UpdateExpense(ExpenseFactory.CreateExpenseEntity(exp));

                if (result.Status == RepositoryActionStatus.Updated)
                {
                    var updatedExpense = ExpenseFactory.CreateExpenseDTO(result.Entity);
                    return Ok(updatedExpense);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}