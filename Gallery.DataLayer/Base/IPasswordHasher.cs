using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.DataLayer.Base
{
    public interface IPasswordHasher<in T>
    {
        string HashPassword(string password, T salt);
        bool IsValid(string password, string hash, T salt);
    }
}
