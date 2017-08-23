using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gallery.Models
{
    public class DiscoveryAlbumViewModel : AlbumViewModel
    {
        public DiscoveryAlbumViewModel()
        {
        }

        public DiscoveryAlbumViewModel(AlbumViewModel model)
        {
            Id = model.Id;
            CreatedAt = model.CreatedAt;
            CreatedBy = model.CreatedBy;
            Name = model.Name;
            Privacy = model.Privacy;
        }

        public string ProfilePicture { get; set; }

        public string UserName { get; set; }

        public string Action { get; set; }

        public IEnumerable<PhotoViewModel> Photos { get; set; }
    }
}