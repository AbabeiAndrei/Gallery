using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Properties;
using Gallery.DataLayer.Repositories;

namespace Gallery.DataLayer.Startup
{
    public static class DependencyConfig
    {
        public static void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterInstance(new DbContext(Settings.Default.ConnectionString))
                   .As<IContext>();
            builder.RegisterType<UserManager>();
            builder.RegisterType<DatabaseUpdater>();
        }
    }
}
