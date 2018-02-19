using AutoMapper;
using PWET.Mappings;
using System.Web.Http;

namespace PWET
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            GlobalConfiguration.Configuration.EnsureInitialized();
        }
    }
}
