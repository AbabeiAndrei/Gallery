using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Model;

namespace Gallery.DataLayer.Entities.Base
{
    public abstract class IdentificableEntity : IHasId<int>
    {
        public abstract int Id { get; set; }
    }
}
