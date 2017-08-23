using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;
using Autofac;

namespace Gallery.Managers
{
    public class AutofacDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            object resolved;
            _container.TryResolve(serviceType, out resolved);
            return resolved;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            object resolved;
            Type type = typeof(IEnumerable<>).MakeGenericType(serviceType);
            _container.TryResolve(type, out resolved);
            return (IEnumerable<object>)resolved;
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public AutofacDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public void Dispose()
        {
        }

        private readonly IContainer _container;
    }
}