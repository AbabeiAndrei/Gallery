using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Entities.Base;
using ServiceStack.DataAnnotations;
using ServiceStack.Model;
using ServiceStack.OrmLite;

namespace Gallery.DataLayer.Entities
{
    public enum PhotoPrivacy : short
    {
        Public = 0,
        Album = 1,
        Private = 2
    }

    [Alias("photos")]
    public class Photo : Entity, IHasId<int>
    {
        [Alias("id")]
        [AutoIncrement]
        public int Id { get; set; }

        [Required]
        [Alias("name")]
        [StringLength(255)]
        public string Name { get; set; }
        
        [Alias("album_id")]
        [References(typeof(Album))]
        public int? AlbumId { get; set; }

        [Required]
        [Alias("uploaded_at")]
        [Default(OrmLiteVariables.SystemUtc)]
        public DateTime UploadedAt { get; set; }

        [Required]
        [Alias("uploaded_by")]
        [References(typeof(User))]
        public int UploadedBy { get; set; }

        [Required]
        [Alias("privacy")]
        public PhotoPrivacy Privacy { get; set; }

        [Required]
        [Alias("file_id")]
        [References(typeof(File))]
        public int FileId { get; set; }

        [Required]
        [Alias("row_state")]
        [Default((int)RowState.Created)]
        public override RowState RowState { get; set; }

        [Pure]
        public override bool HasAccess(IIdentity identity, Operation operation, object data = null)
        {
            if (UploadedBy == identity.Id)
                return true;

            if (operation == Operation.Read && Privacy == PhotoPrivacy.Public)
                return true;

            if (operation != Operation.Read || Privacy != PhotoPrivacy.Album)
                return UploadedBy == identity.Id;

            var album = data as Album;
            return album != null && album.HasAccess(identity, Operation.Read);
        }
    }
}
