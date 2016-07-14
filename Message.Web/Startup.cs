using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Message.Web.Startup))]
namespace Message.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
