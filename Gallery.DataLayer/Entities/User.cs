using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Entities.Base;

namespace Gallery.DataLayer.Entities
{
    public class User : IEntity
    {
        public RowState RowState { get; set; }
    }
}
