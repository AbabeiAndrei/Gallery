using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using Autofac;
using Gallery.DataLayer.Startup;

namespace Gallery
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            var container = DependencyConfig.RegisterDependencies(GlobalConfiguration.Configuration);  
            DatabaseConfig.Config(container.Resolve<DatabaseUpdater>());
        }
    }
}