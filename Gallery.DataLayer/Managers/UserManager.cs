using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities;

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
            return _context.GetAll<User>();
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
    }
}
