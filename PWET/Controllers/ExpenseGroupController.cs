using AutoMapper;
using PWET.DTO;
using PWET.Mappings;
using PWET.Repo;
using System;
using System.Collections.Generic;
using System.Web.Http;
using PWET.Helpers;
using System.Linq;
using PWET.Repo.Factories;
using PWET.Repo.EFstatus;
using System.Net;
using System.Web.Http.Routing;
using System.Web;
using Newtonsoft.Json;
using Marvin.JsonPatch;
using PWET.Domain;

namespace PWET.Controllers
{
    public class ExpenseGroupController : ApiController
    {
        private readonly IExpenseTrackerRepository _repo;
        const int MaxPageItems = 5;

        public ExpenseGroupController()
        {
            _repo = new ExpenseTrackerRepository(new Repo.Context.PWETContext());
        }

        public ExpenseGroupController(IExpenseTrackerRepository repo)
        {
            _repo = repo;
        }

        [Route("api/expensegroup", Name = "ExpenseGroupsList")]
        public IHttpActionResult Get(string fields = null, string sort = "id", string status = null, string userId = null, int page = 1, int pageSize = MaxPageItems)
        {
            List<string> lstOfFields = new List<string>();

            if (fields != null)
            {
                lstOfFields = fields.ToLower().Split(',').ToList();
            }

            //conversion of status to their (int)id value 
            int statusId = -1;
            if (status != null)
            {
                switch (status.ToLower())
                {
                    case "open":
                        statusId = 1;
                        break;
                    case "confirmed":
                        statusId = 2;
                        break;
                    case "processed":
                        statusId = 3;
                        break;
                    default:
                        break;
                }
            }

            IEnumerable<ExpenseGroup> lstexpenseGroups = _repo.GetExpenseGroupsWithExpenses()
                .Where(eg => (statusId == -1) || (eg.ExpenseGroupStatusId == statusId))
                .ApplySort(sort);

            var totalCount = lstexpenseGroups.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (pageSize > MaxPageItems)
            {
                pageSize = MaxPageItems;
            }

            var urlHelper = new UrlHelper(Request);
            var prevLink = page > 1 ? urlHelper.Link("ExpenseGroupsList",
                new
                {
                    page = page - 1,
                    pageSize = pageSize,
                    sort = sort

                    //  fields = fields
                    ,
                    status = status,
                    userId = userId
                }) : "";
            var nextLink = page < totalPages ? urlHelper.Link("ExpenseGroupsList",
                new
                {
                    page = page + 1,
                    pageSize = pageSize,
                    sort = sort

                     //   fields = fields
                     ,
                    status = status,
                    userId = userId
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

            HttpContext.Current.Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationHeader));

            var config = new AutoMapperConfiguration().Configure();
            IMapper mapper = config.CreateMapper();
            var dest = mapper.Map<IEnumerable<ExpenseGroup>, IEnumerable<ExpenseGroupDTO>>(lstexpenseGroups);

            try
            {
                return Ok(dest.Skip(pageSize * (page - 1)).Take(pageSize));
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        public IHttpActionResult Get(int id, string fields = null)
        {
            try
            {
                bool includeExpenses = false;
                List<string> lstOfFields = new List<string>();

                // we should include expenses when the fields-string contains "expenses"
                if (fields != null)
                {
                    lstOfFields = fields.ToLower().Split(',').ToList();
                    includeExpenses = lstOfFields.Any(f => f.Contains("expenses"));
                }

                ExpenseGroup expenseGroup = null;
                if (includeExpenses)
                {
                    expenseGroup = _repo.GetExpenseGroupWithExpenses(id);
                }
                else
                {
                    expenseGroup = _repo.GetExpenseGroup(id);
                }

                if (expenseGroup != null)
                {
                    return Ok(ExpenseGroupFactory.CreateDataShapedObjectExpenseGroup(expenseGroup, lstOfFields));
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

        [Route("api/expensegroup")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] ExpenseGroupDTO expensegroup)
        {
            try
            {
                if (expensegroup == null)
                {
                    return BadRequest();
                }
                else
                {
                    //no reverse mapping with AutoMap
                    var eg = ExpenseGroupFactory.CreateExpenseGroupEntity(expensegroup);

                    var result = _repo.InsertExpenseGroup(eg);
                    if (result.Status == RepositoryActionStatus.Created)
                    {
                        var newExpenseGroup = ExpenseGroupFactory.CreateExpenseGroupDTO(result.Entity);
                        return Created<ExpenseGroupDTO>(Request.RequestUri + "/" + newExpenseGroup.Id.ToString(), newExpenseGroup);
                    }
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody]ExpenseGroupDTO expensegroup)
        {
            try
            {
                if (expensegroup == null)
                {
                    return BadRequest();
                }
                var eg = ExpenseGroupFactory.CreateExpenseGroupEntity(expensegroup);

                var result = _repo.UpdateExpenseGroup(eg);
                if (result.Status == RepositoryActionStatus.Updated)
                {
                    var updatedExpenseGroup = ExpenseGroupFactory.CreateExpenseGroupDTO(result.Entity);
                    return Ok(updatedExpenseGroup);
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

        public IHttpActionResult Delete(int id)
        {
            try
            {
                var result = _repo.DeleteExpenseGroup(id);

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
        public IHttpActionResult Patch(int id, [FromBody]JsonPatchDocument<ExpenseGroupDTO> expenseGroupPatchDocument)
        {
            try
            {
                if (expenseGroupPatchDocument == null)
                {
                    return BadRequest();
                }

                var expenseGroup = _repo.GetExpenseGroup(id);
                if (expenseGroup == null)
                {
                    return NotFound();
                }

                var eg = ExpenseGroupFactory.CreateExpenseGroupDTO(expenseGroup);
                expenseGroupPatchDocument.ApplyTo(eg);
                var result = _repo.UpdateExpenseGroup(ExpenseGroupFactory.CreateExpenseGroupEntity(eg));

                if (result.Status == RepositoryActionStatus.Updated)
                {
                    var patchedExpenseGroup = ExpenseGroupFactory.CreateExpenseGroupDTO(result.Entity);
                    return Ok(patchedExpenseGroup);
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

