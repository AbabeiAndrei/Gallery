using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Gallery.DataLayer.Entities;

namespace Gallery.Models
{
    public class MultiplePhotoSaveViewModel
    {
        [Required]
        public IEnumerable<int> PhotoIds { get; set; }

        [Required]
        public int AlbumId { get; set; }

        [Required]
        public PhotoPrivacy Privacy { get; set; }
    }
}