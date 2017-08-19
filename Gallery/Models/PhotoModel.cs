using Gallery.DataLayer.Entities;

namespace Gallery.Models
{
    public class PhotoModel
    {
        public string Name { get; set; }

        public int? AlbumId { get; set; }

        public PhotoPrivacy Privacy { get; set; }

        public int FileId { get; set; }
    }
}