using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Entities.Base;

namespace Gallery.DataLayer.Managers
{
    public class FileManager : IManager<File>
    {
        private readonly IContext _context;

        public FileManager(IContext context)
        {
            _context = context;
        }

        public IEnumerable<File> GetAll()
        {
            return _context.GetAll<File>(f => f.RowState != RowState.Deleted);
        }

        public File GetById(object id)
        {
            return _context.GetById<File>(id);
        }

        public void Add(File item)
        {
            _context.Add(item);
        }

        public void Update(File item)
        {
            _context.Update(item);
        }

        public void Delete(object id)
        {
            _context.Delete<File>(id);
        }
    }
}
