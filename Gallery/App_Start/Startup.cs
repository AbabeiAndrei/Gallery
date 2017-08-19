using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Owin;

namespace Gallery
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            appBuilder.UseWebApi(httpConfiguration);
        }
    }
}