using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CmsSite.Startup))]
namespace CmsSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
