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
    [EnumAsInt]
    public enum AlbumPrivacy : short
    {
        Public = 0,
        Private = 1
    }

    [Alias("albums")]
    public class Album : Entity
    {
        [Alias("id")]
        [AutoIncrement]
        public override int Id { get; set; }

        [Required]
        [Alias("name")]
        [StringLength(255)]
        public string Name { get; set; }
        
        [Required]
        [Alias("created_at")]
        [Default(OrmLiteVariables.SystemUtc)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Alias("created_by")]
        [References(typeof(User))]
        public int CreatedBy { get; set; }

        [Required]
        [Alias("privacy")]
        public AlbumPrivacy Privacy { get; set; }

        [Required]
        [Alias("row_state")]
        [Default((int)RowState.Created)]
        public override RowState RowState { get; set; }

        [Pure]
        public override bool HasAccess(IIdentity identity, Operation operation, object data = null)
        {
            if(operation == Operation.Read)
                return Privacy == AlbumPrivacy.Public || CreatedBy == identity.Id;

            return CreatedBy == identity.Id;
        }
    }
}
