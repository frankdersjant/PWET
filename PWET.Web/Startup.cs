using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PWET.Web.Startup))]
namespace PWET.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
