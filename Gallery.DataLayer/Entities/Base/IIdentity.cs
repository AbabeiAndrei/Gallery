using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Model;

namespace Gallery.DataLayer.Entities.Base
{
    public interface IIdentity : IHasId<int>
    {
        UserRole Role { get; }
    }
}
