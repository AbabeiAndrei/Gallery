using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Gallery.Managers;

namespace Gallery
{
    public static class DependencyConfig
    {
        public static IContainer RegisterDependencies(HttpConfiguration configuration)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<AuthenticationManager>();

            DataLayer.Startup.DependencyConfig.RegisterDependencies(builder);

            var container = builder.Build();

            var resolver = new AutofacWebApiDependencyResolver(container);

            configuration.DependencyResolver = resolver;
            
            GlobalConfiguration.Configuration.DependencyResolver = resolver;

            return container;
        }


    }
}