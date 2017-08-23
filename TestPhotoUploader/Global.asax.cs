using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TestPhotoUploader.App_Start;

namespace TestPhotoUploader
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            SimpleInjectorInitializer.Initialize();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}