using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gallery.DataLayer.Entities;

namespace Gallery.Models
{
    public class PhotoViewModel : PhotoModel
    {
        public int Id { get; set; }

        public DateTime UploadedAt { get; set; }

        public int UploadedBy { get; set; }
    }
}