using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gallery.Models
{
    public class FileViewModel
    {
        public int Id { get; set; }
        
        public string FilePath { get; set; }
        
        public string ThumbnailPath { get; set; }
       
        public string Extension { get; set; }
        
        public int UploadedBy { get; set; }
        
        public DateTime UploadedAt { get; set; }
        
        public int Size { get; set; }
    }
}