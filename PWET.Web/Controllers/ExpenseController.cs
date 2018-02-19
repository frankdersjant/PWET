using Marvin.JsonPatch;
using Newtonsoft.Json;
using PWET.Domain;
using PWET.DTO;
using PWET.Repo.Helpers;
using PWET.Web.Helpers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PWET.Web.Controllers
{
    public class ExpenseController : Controller
    {
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Expense expense)
        {
            try
            {
                var client = PWETHttpClient.GetClient();
                var serializedItemToCreate = JsonConvert.SerializeObject(expense);
                HttpResponseMessage response = await client.PostAsync("api/expenses", new StringContent(serializedItemToCreate, System.Text.Encoding.Unicode, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details", "ExpenseGroup", new { id = expense.ExpenseGroupId });
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

        public async Task<ActionResult> Edit(int id)
        {
            var client = PWETHttpClient.GetClient();

            HttpResponseMessage response = await client.GetAsync("api/expenses/" + id + "?fields=id,description,date,amount");

            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<Expense>(content);
                return View(model);
            }
            else
            {
                return Content("An error occurred");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Expense expense)
        {
            try
            {
                var client = PWETHttpClient.GetClient();

                JsonPatchDocument<ExpenseDTO> patchDoc = new JsonPatchDocument<ExpenseDTO>();
                patchDoc.Replace(e => e.Amount, expense.Amount);
                patchDoc.Replace(e => e.Date, expense.Date);
                patchDoc.Replace(e => e.Description, expense.Description);

                var serializedItemToUpdate = JsonConvert.SerializeObject(patchDoc);
                HttpResponseMessage response = await client.PatchAsync("api/expenses/" + id, new StringContent(serializedItemToUpdate, System.Text.Encoding.Unicode, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details", "ExpenseGroup", new { id = expense.ExpenseGroupId });
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

        public async Task<ActionResult> Delete(int expensegroupId, int id)
        {
            try
            {
                var client = PWETHttpClient.GetClient();

                HttpResponseMessage response = await client.DeleteAsync("api/expenses/" + id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details", "ExpenseGroup", new { id = expensegroupId });
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
    }
}