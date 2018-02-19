using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebAppPWET.Startup))]
namespace WebAppPWET
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
