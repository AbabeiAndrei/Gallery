using System;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using AutoMapper;
using Gallery.DataLayer.Startup;
using Gallery.Managers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(Gallery.Startup))]

namespace Gallery
{
    public class Startup
    {
        public const string SITE_LOCATION = "http://gallery.andrei.local/";

        public static IContainer Resolver { get; private set; }

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
            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);


            Resolver = DependencyConfig.RegisterDependencies(GlobalConfiguration.Configuration, app);
            DatabaseConfig.Config(Resolver.Resolve<DatabaseUpdater>());

            Mapper.Initialize(MappingConfig.CreateConfiguration);
        }
    }
}
