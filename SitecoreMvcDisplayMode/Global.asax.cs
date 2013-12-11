using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;

using SitecoreMvcDisplayMode.Utility;

namespace SitecoreMvcDisplayMode
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BrowserOverrideStores.Current = new SitecoreBrowserOverrideStore();
        }
    }
}
