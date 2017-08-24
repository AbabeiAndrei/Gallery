using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gallery.Models
{
    public class DiscoveryPhotoViewModel : PhotoViewModel
    {
        public DiscoveryPhotoViewModel()
        {
        }

        public DiscoveryPhotoViewModel(PhotoViewModel pvm)
            : this()
        {
            Id = pvm.Id;
            AlbumId = pvm.AlbumId;
            Name = pvm.Name;
            FileId = pvm.FileId;
            Privacy = pvm.Privacy;
            UploadedAt = pvm.UploadedAt;
            UploadedBy = pvm.UploadedBy;
        }

        public string Url { get; set; }

        public int OtherX { get; set; }
    }
}