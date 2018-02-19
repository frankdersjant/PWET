using Newtonsoft.Json;
using PWET.Repo;
using PWET.Repo.Entities;
using PWET.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PWET.Web.Controllers
{
    public class HomeController : Controller
    {

        public async Task<ActionResult> Index()
        {
            var client = PWETHttpClient.GetClient();

           HttpResponseMessage egsResponse = await client.GetAsync("api/expensegroup");

            if (egsResponse.IsSuccessStatusCode)
            {
                string egsContent = await egsResponse.Content.ReadAsStringAsync();
                var lstExpenses = JsonConvert.DeserializeObject<Expense>(egsContent);
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}