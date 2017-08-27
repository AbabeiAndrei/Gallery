using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Gallery.DataLayer;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Managers;
using Gallery.DataLayer.Startup;
using Gallery.DataLayer.Utils;
using Gallery.Managers;
using Gallery.Properties;
using Owin;

namespace Gallery
{
    public static class DependencyConfig
    {
        private static IContainer _container;

        public static IContainer RegisterDependencies(HttpConfiguration configuration, IAppBuilder app = null)
        {
            var httpConfig = new HttpConfiguration();
            
            ConfigureComposition(httpConfig, app);

            GlobalConf(httpConfig, app);

            return _container;
        }

        private static void ConfigureComposition(HttpConfiguration config, IAppBuilder app)
        {
            IContainer container = GetServiceContainer(config, app);

            config.DependencyResolver = new Managers.AutofacDependencyResolver(container);
        }

        private static void GlobalConf(HttpConfiguration config, IAppBuilder app)
        {
            var container = GetServiceContainer(config, app);
            var resolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }

        public static IContainer GetServiceContainer(HttpConfiguration config = null, IAppBuilder app = null)
        {
            if (_container != null)
            {
                return _container;
            }

            var builder = CreateContainerBuilder(Assembly.GetExecutingAssembly());

            _container = builder.Build();

            app.UseAutofacMiddleware(_container);
            app.UseAutofacWebApi(config);

            return _container;
        }

        public static ContainerBuilder CreateContainerBuilder(Assembly assembly)
        {
            var builder = new ContainerBuilder();
            RegisterGenericComponents(builder);
            RegisterHttpComponents(assembly, builder);

            return builder;
        }

        private static void RegisterGenericComponents(ContainerBuilder builder)
        {
            builder.RegisterInstance(new DbContext(Settings.Default.ConnectionString))
                   .As<IContext>();
            builder.RegisterType<UserPasswordHasher>()
                   .As<IPasswordHasher<User>>();

            builder.RegisterType<AuthenticationManager>();
            builder.RegisterType<UserManager>();
            builder.RegisterType<FileManager>();
            builder.RegisterType<AlbumManager>();
            builder.RegisterType<PhotoManager>();
            builder.RegisterType<DatabaseUpdater>();
        }

        private static void RegisterHttpComponents(Assembly assembly, ContainerBuilder builder)
        {
            // register all WebAPI controllers
            builder.RegisterApiControllers(assembly);

            // register all MVC controllers
            builder.RegisterControllers(assembly);
        }
        
    }
}