using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gallery.Models
{
    public class BaseModel
    {
        protected BaseModel()
        {
            
        }

        public bool IsValid()
        {
            var type = GetType();

            return true;
        }
    }
}