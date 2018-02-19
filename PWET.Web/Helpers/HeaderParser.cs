using Newtonsoft.Json;
using System.Linq;
using System.Net.Http.Headers;

namespace PWET.Web.Helpers
{
    public static class HeaderParser
    {
        public static PagingInfo FindAndParsePagingInfo(HttpResponseHeaders responseHeaders)
        {
            //added in controller
            if (responseHeaders.Contains("X-Pagination"))
            {
                var xPag = responseHeaders.First(ph => ph.Key == "X-Pagination").Value;
                return JsonConvert.DeserializeObject<PagingInfo>(xPag.First());
            }
            return null;
        }
    }
}