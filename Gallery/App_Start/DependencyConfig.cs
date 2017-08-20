using System.Reflection;
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

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<AuthenticationManager>();

            DataLayer.Startup.DependencyConfig.RegisterDependencies(builder);

            builder.RegisterWebApiFilterProvider(configuration);
            builder.RegisterWebApiModelBinderProvider();

            var container = builder.Build();

            var resolver = new AutofacWebApiDependencyResolver(container);

            configuration.DependencyResolver = resolver;
            
            return container;
        }


    }
}