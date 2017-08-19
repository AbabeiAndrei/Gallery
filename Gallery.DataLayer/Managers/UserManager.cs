using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Entities.Base;
using ServiceStack.OrmLite;

namespace Gallery.DataLayer.Repositories
{
    public class UserManager : IManager<User>
    {
        private readonly IContext _context;
        private readonly IPasswordHasher<User> _hasher;

        public UserManager(IContext context, IPasswordHasher<User> hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.GetAll<User>(u => u.RowState != RowState.Deleted);
        }

        public User GetById(object id)
        {
            return _context.GetById<User>(id);
        }

        public void Add(User item)
        {
            _context.Add(item);
        }

        public void Update(User item)
        {
            _context.Update(item);
        }

        public void Delete(object id)
        {
            _context.Delete<User>(id);
        }

        public User Login(string email, string password)
        {
            using (var connection = _context.Connection)
            {
                var user = connection.Select<User>(u => u.Email == email && u.RowState != RowState.Deleted)
                                     .FirstOrDefault();

                if (user == null)
                    return null;

                if (!_hasher.IsValid(password, user.Password, user))
                    return null;

                return user;
            }
        }

        public bool Exist(string email)
        {
            using (var connection = _context.Connection)
                return connection.Exists<User>(u => u.Email == email && u.RowState != RowState.Deleted);
        }
    }
}
