using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Gallery.Extensions
{
    public static class GenericEx
    {
        public static T Clone<T>(this T item)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(item));
        }
    }
}