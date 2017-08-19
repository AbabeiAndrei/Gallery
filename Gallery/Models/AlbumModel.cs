using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Gallery.DataLayer.Entities;

namespace Gallery.Models
{
    public class AlbumModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public AlbumPrivacy Privacy { get; set; }
    }
}