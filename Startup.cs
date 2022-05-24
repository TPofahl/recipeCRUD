using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RecipeCRUD.Startup))]
namespace RecipeCRUD
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
