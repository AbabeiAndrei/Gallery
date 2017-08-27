using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gallery.Models
{
    public class MultiplePhotoUploadViewModel
    {
        public IEnumerable<PhotoModel> Photos { get; set; }

        public int AlbumId { get; set; }
    }
}