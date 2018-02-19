using PWET.Constants;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PWET.Web.Helpers
{
    public static class PWETHttpClient
    {

        public static HttpClient GetClient(string requestedVersion = null)
        {

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(PWEConstants.PWEAPI);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            //versioning
            //if (requestedVersion != null)
            //{
            //    // through a custom request header
            //    //client.DefaultRequestHeaders.Add("api-version", requestedVersion);

            //    // through content negotiation
            //    client.DefaultRequestHeaders.Accept.Add(
            //        new MediaTypeWithQualityHeaderValue("application/vnd.expensetrackerapi.v"
            //            + requestedVersion + "+json"));
            //}

            return client;
        }
    }
}
