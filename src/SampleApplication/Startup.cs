using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SampleApplication.Startup))]
namespace SampleApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
