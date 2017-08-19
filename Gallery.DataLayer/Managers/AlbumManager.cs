using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Entities.Base;

namespace Gallery.DataLayer.Managers
{
    public class AlbumManager : IManager<Album>
    {
        private readonly IContext _context;

        public AlbumManager(IContext context)
        {
            _context = context;
        }

        [Pure]
        public IEnumerable<Album> GetAll()
        {
            return _context.GetAll<Album>(a => a.RowState != RowState.Deleted);
        }

        [Pure]
        public Album GetById(object id)
        {
            return _context.GetById<Album>(id);
        }

        public void Add(Album item)
        {
            _context.Add(item);
        }

        public void Update(Album item)
        {
            _context.Update(item);
        }

        public void Delete(object id)
        {
            _context.Delete<Album>(id);
        }

        public IEnumerable<Album> GetUserAlbums(int userId)
        {
            return _context.GetAll<Album>(a => a.CreatedBy == userId && a.RowState != RowState.Deleted);
        }
    }
}
