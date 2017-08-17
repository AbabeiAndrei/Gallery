using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Entities;

namespace Gallery.DataLayer.Base
{
    public interface IDbContext
    {
        IEnumerable<T> GetAll<T>(Expression<Func<T, bool>> filter = null);
        T GetById<T>(object id);
        void Add<T>(T item);
        void Update<T>(T item);
        void Delete<T>(object id);
    }
}
