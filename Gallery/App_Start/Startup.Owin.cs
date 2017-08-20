using System;
using System.Threading.Tasks;
using System.Web.Http;
using Gallery.Managers;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(Gallery.Startup))]

namespace Gallery
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            app.UseWebApi(httpConfiguration);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
                                               {
                                                   AuthenticationType = AuthenticationManager.APPLICATION_COKIE_AUTH,
                                                   LoginPath = new PathString("/Gallery/Login")
                                               });
        }
    }
}
