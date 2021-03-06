﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Entities.Base;

namespace Gallery.DataLayer.Managers
{
    public class PhotoManager : IManager<Photo>
    {
        private readonly IContext _context;

        public PhotoManager(IContext context)
        {
            _context = context;
        }

        public IEnumerable<Photo> GetAll()
        {
            return _context.GetAll<Photo>(p => p.RowState != RowState.Deleted);
        }

        public IEnumerable<Photo> GetAll(IEnumerable<int> fromId)
        {
            return _context.GetAll<Photo>(p => fromId.Contains(p.Id));
        }

        public Photo GetById(int id)
        {
            return _context.GetById<Photo>(id);
        }

        public void Add(Photo item)
        {
            _context.Add(item);
        }

        public void Update(Photo item)
        {
            _context.Update(item);
        }

        public void Delete(int id)
        {
            _context.Delete<Photo>(id);
        }

        public IEnumerable<Photo> GetUserPhotos(int userId)
        {
            return _context.GetAll<Photo>(p => p.UploadedBy == userId && p.RowState != RowState.Deleted);
        }

        public IEnumerable<Photo> GetAlbumPhotos(int albumId)
        {
            return _context.GetAll<Photo>(p => p.AlbumId == albumId && p.RowState != RowState.Deleted);
        }
    }
}
