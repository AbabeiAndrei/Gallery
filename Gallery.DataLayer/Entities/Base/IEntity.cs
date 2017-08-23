using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace Gallery.DataLayer.Entities.Base
{
    [EnumAsInt]
    public enum RowState : short
    {
        Created = 0,
        Deleted = 1
    }

    public enum Operation : short
    {
        Read,
        Update,
        Delete
    }

    public interface IEntity
    {
        RowState RowState { get; set; }
        bool HasAccess(IIdentity identity, Operation operation, object data = null);
    }
}
