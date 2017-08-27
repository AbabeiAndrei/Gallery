using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gallery.Models
{
    public class ResultFileUploadModel
    {
        [Required]
        public string Extension { get; set; }

        [Required]
        public string FileContent { get; set; }
    }
}