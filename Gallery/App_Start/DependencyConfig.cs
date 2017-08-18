using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;

namespace Gallery
{
    public static class DependencyConfig
    {
        public static IContainer RegisterDependencies(HttpConfiguration configuration)
        {
            ContainerBuilder builder = new ContainerBuilder();

            DataLayer.Startup.DependencyConfig.RegisterDependencies(builder);

            IContainer container = builder.Build();

            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            return container;
        }


    }
}