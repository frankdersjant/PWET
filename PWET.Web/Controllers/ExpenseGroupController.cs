using Marvin.JsonPatch;
using Newtonsoft.Json;
using PagedList;
using PWET.Domain;
using PWET.DTO;
using PWET.Repo.Helpers;
using PWET.Web.Helpers;
using PWET.Web.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PWET.Web.Controllers
{
    public class ExpenseGroupController : Controller
    {

        public async Task<ActionResult> Index(int? page=1)
        {
            var ExpenseGroupsVM = new ExpenseGroupsViewModel();

            var client = PWETHttpClient.GetClient();

            //Statusses
            HttpResponseMessage egssResponse = await client.GetAsync("api/expensegroupstatusses");

            if (egssResponse.IsSuccessStatusCode)
            {
                string egsContent = await egssResponse.Content.ReadAsStringAsync();
                var lstExpensesGroupsStatus = JsonConvert.DeserializeObject<IEnumerable<ExpenseGroupStatus>>(egsContent);

                ExpenseGroupsVM.ExpenseGroupStatus = lstExpensesGroupsStatus;
            }
            else
            {
                return Content("An error has occurred");
            }

            //Expenses
            HttpResponseMessage Response = await client.GetAsync("api/expensegroup?sort=expensegroupstatusid,title&page=" + page + "&pagesize=5");

            if (Response.IsSuccessStatusCode)
            {
                string content = await Response.Content.ReadAsStringAsync();

                var pagingInfo = HeaderParser.FindAndParsePagingInfo(Response.Headers);

                var lstExpenseGroups = JsonConvert.DeserializeObject<IEnumerable<ExpenseGroup>>(content);
                var pagedListExpenseGroups = new StaticPagedList<ExpenseGroup>(lstExpenseGroups, pagingInfo.CurrentPage, pagingInfo.PageSize, pagingInfo.TotalCount);

                ExpenseGroupsVM.ExpenseGroup = pagedListExpenseGroups;
                ExpenseGroupsVM.PagingInfo = pagingInfo;
            }
            else
            {
                return Content("An error occurred.");
            }

            return View(ExpenseGroupsVM);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ExpenseGroup expensegroup)
        {
            try
            {
                var client = PWETHttpClient.GetClient();

                // an expensegroup is created with status "Open", for the current user
                expensegroup.ExpenseGroupStatusId = 1;
                expensegroup.UserId = "IReallyDontKnow";
                var serializedItemToCreate = JsonConvert.SerializeObject(expensegroup);

                //PostAsync
                var response = await client.PostAsync("api/expensegroup", new StringContent(serializedItemToCreate, System.Text.Encoding.Unicode, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("An error occurred");
                }
            }
            catch
            {
                return Content("An error occurred.");
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            var client = PWETHttpClient.GetClient();

            HttpResponseMessage response = await client.GetAsync("api/expensegroup/" + id + "?fields=id,title,description");
            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<ExpenseGroup>(content);
                return View(model);
            }
            else
            {
                return Content("An error occurred");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ExpenseGroup expensegroup)
        {
            try
            {
                var client = PWETHttpClient.GetClient();

                JsonPatchDocument<ExpenseGroupDTO> patchDoc = new JsonPatchDocument<ExpenseGroupDTO>();
                patchDoc.Replace(eg => eg.Title, expensegroup.Title);
                patchDoc.Replace(eg => eg.Description, expensegroup.Description);
                var serializedItemToUpdate = JsonConvert.SerializeObject(patchDoc);

                var response = await client.PatchAsync("api/expensegroup/" + id, new StringContent(serializedItemToUpdate, System.Text.Encoding.Unicode, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("An error occurred");
                }
            }
            catch
            {
                return Content("An error occurred.");
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var client = PWETHttpClient.GetClient();

                var response = await client.DeleteAsync("api/expensegroup/" + id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("An error occurred");
                }
            }
            catch
            {
                return Content("An error occurred");
            }
        }

        public  async Task<ActionResult> Details(int id)
        {
            var client = PWETHttpClient.GetClient();

            HttpResponseMessage response = await client.GetAsync("api/expensegroup/" + id + "?fields=id,description,title,expenses");
            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<ExpenseGroup>(content);
                return View(model);
            }

            return Content("An error occured");
        }
    }
}