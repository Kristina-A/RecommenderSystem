using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RecommenderSystem.Startup))]
namespace RecommenderSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
