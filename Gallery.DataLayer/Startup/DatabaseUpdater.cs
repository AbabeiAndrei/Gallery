using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallery.DataLayer.Base;
using Gallery.DataLayer.Entities;
using Gallery.DataLayer.Entities.Base;
using ServiceStack.OrmLite;

namespace Gallery.DataLayer.Startup
{
    public class DatabaseUpdater
    {
        private readonly IContext _context;
        private readonly IPasswordHasher<User> _hasher;

        public DatabaseUpdater(IContext context, IPasswordHasher<User> hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        public void UpdateDatabase()
        {
            using (var connection = _context.Connection)
            {
                connection.CreateTableIfNotExists<User>();
                connection.CreateTableIfNotExists<File>();
                connection.CreateTableIfNotExists<Album>();
                connection.CreateTableIfNotExists<Photo>();
            }
        }

        public async Task SeedData()
        {
            using (var connection = _context.Connection)
            {
                if (!connection.Exists<User>(u => u.RowState != RowState.Deleted))
                {
                    var user = CreateAdminUser();
                    await connection.InsertAsync(user);
                }
            }
        }

        private User CreateAdminUser()
        {
            var user = new User
            {
                CreatedAt = DateTime.Now,
                Email = "admin@local.test",
                FullName = "Administrator",
                RowState = RowState.Created
            };

            user.Password = _hasher.HashPassword("password", user);

            return user;
        }
    }
}
