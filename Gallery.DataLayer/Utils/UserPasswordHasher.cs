using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities;

namespace Gallery.DataLayer.Utils
{
    public class UserPasswordHasher : IPasswordHasher<User>
    {
        public string HashPassword(string password, User salt)
        {
            var crypt = new SHA256Managed();
            var passWithSalt = password + "-" + salt.Email.GetHashCode();

            var hash = string.Empty;
            var crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(passWithSalt), 0, Encoding.ASCII.GetByteCount(passWithSalt));

            foreach (var @byte in crypto)
                hash += @byte.ToString("x2");

            return hash;
        }

        public bool IsValid(string password, string hash, User salt)
        {
            return HashPassword(password, salt) == hash;
        }
    }
}
