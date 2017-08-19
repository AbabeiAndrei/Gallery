using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gallery.DataLayer.Entities;

namespace Gallery.Models
{
    public class AlbumViewModel : AlbumModel
    {
        public int Id { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public int CreatedBy { get; set; }
    }
}