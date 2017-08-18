using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities.Base;
using ServiceStack.OrmLite;

namespace Gallery.DataLayer
{
    public class DbContext : IContext
    {
        private readonly OrmLiteConnectionFactory _dbFactory;

        public IDbConnection Connection => _dbFactory.OpenDbConnection();

        public DbContext(string connectionString)
        {
            _dbFactory = new OrmLiteConnectionFactory(connectionString, MySqlDialect.Provider);
        }

        public IEnumerable<T> GetAll<T>(Expression<Func<T, bool>> filter = null)
        {
            using (var connection = Connection)
            { 
                return connection.Select(filter);
            }
        }

        public T GetById<T>(object id)
        {
            using (var connection = Connection)
            {
                return connection.SingleById<T>(id);
            }
        }

        public void Add<T>(T item)
        {
            using (var connection = Connection)
            {
                connection.Insert(item);
            }
        }

        public void Update<T>(T item)
        {
            using (var connection = Connection)
            {
                connection.Update(item);
            }
        }

        public void Delete<T>(object id)
        {
            using (var connection = Connection)
            {
                var item = connection.SingleById<T>(id);
                var entity = item as IEntity;

                if (entity == null)
                {
                    connection.Delete(item);
                }
                else if (entity.RowState != RowState.Deleted)
                {
                    entity.RowState = RowState.Deleted;
                    connection.Update(item);
                }
            }
        }
    }
}
