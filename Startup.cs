using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CLIP.Startup))]
namespace CLIP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
