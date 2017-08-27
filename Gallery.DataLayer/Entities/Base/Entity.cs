using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.DataLayer.Entities.Base
{
    public abstract class Entity : IdentificableEntity, IEntity
    {
        public abstract RowState RowState { get; set; }

        [Pure]
        public virtual bool HasAccess(IIdentity identity, Operation operation, object data = null) => true;
    }
}
