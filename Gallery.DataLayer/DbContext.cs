using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Base;

namespace Gallery.DataLayer
{
    public class DbContext : IDbContext
    {
        public DbContext(string connectionString)
        {

        }

        public IEnumerable<T> GetAll<T>(Expression<Func<T, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public T GetById<T>(object id)
        {
            throw new NotImplementedException();
        }

        public void Add<T>(T item)
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T item)
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(object id)
        {
            throw new NotImplementedException();
        }
    }
}
