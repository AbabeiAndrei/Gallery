using Autofac;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Managers;
using Gallery.DataLayer.Properties;
using Gallery.DataLayer.Repositories;
using Gallery.DataLayer.Utils;

namespace Gallery.DataLayer.Startup
{
    public static class DependencyConfig
    {
        public static void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterInstance(new DbContext(Settings.Default.ConnectionString))
                   .As<IContext>();
            builder.RegisterType<UserPasswordHasher>()
                   .As<IPasswordHasher<User>>();
            builder.RegisterType<UserManager>();
            builder.RegisterType<FileManager>();
            builder.RegisterType<AlbumManager>();
            builder.RegisterType<PhotoManager>();
            builder.RegisterType<DatabaseUpdater>();
        }
    }
}
