using MonoTask.MVC.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using log4net.Config;

namespace MonoTask.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalFilters.Filters.Add(new CustomExceptionFilter());
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            XmlConfigurator.Configure();

        }
    }
}
