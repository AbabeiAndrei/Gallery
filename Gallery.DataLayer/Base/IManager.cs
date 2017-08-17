using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.DataLayer.Base
{
    public interface IManager<T>
    {
        IEnumerable<T> GetAll();
        T GetById(object id);
        void Add(T item);
        void Update(T item);
        void Delete(object id);
    }
}
