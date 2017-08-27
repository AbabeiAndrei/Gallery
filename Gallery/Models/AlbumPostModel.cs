using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gallery.Models
{
    public class AlbumPostModel : AlbumModel
    {
        public IEnumerable<PhotoModel> Photos { get; set; }
    }
}