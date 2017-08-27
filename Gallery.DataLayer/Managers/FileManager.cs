using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities.Base;
using File = Gallery.DataLayer.Entities.File;

namespace Gallery.DataLayer.Managers
{
    public class FileManager : IManager<File>
    {
        public const string RELATIVE_PATH = "app\\upload\\photos";
        public const string RELATIVE_PATH_ARCHIVE = "app\\upload\\archive";
        public const string RELATIVE_PATH_THUMBNAIL = "app\\upload\\photos\\thumbnail";

        private readonly IContext _context;

        public FileManager(IContext context)
        {
            _context = context;
        }

        public IEnumerable<File> GetAll()
        {
            return _context.GetAll<File>(f => f.RowState != RowState.Deleted);
        }

        public IEnumerable<File> GetAll(IEnumerable<int> files)
        {
            return _context.GetAll<File>(f => files.Contains(f.Id));
        }

        public File GetById(int id)
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

        public void Delete(int id)
        {
            _context.Delete<File>(id);
        }

        public string GetLocalPath(File file, string absolutePath)
        {
            return Path.Combine(absolutePath, RELATIVE_PATH, file.Name);
        }
    }
}
