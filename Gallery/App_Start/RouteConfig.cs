using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gallery
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Home",
                            "{controller}/{action}",
                            new { controller = "Gallery", action = "Discover" });
            

            //routes.MapRoute("Default",
            //                "Home/Index/{page}",
            //                new { page = "home" });
        }
    }
}
