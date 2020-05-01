namespace ParrotWingsAPI
{
    using Microsoft.EntityFrameworkCore;
    using ParrotWingsData;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class UsersService : IUsersService
    {
        private readonly Func<ParrotWingsDBContext> _contextFactory;
        private readonly IHashFunction _hash;

        public UsersService(Func<ParrotWingsDBContext> contextFactory, IHashFunction hash)
        {
            _hash = hash;
            _contextFactory = contextFactory;
        }

        public async Task<bool> AddUserAsync(RegisterCredentials credentials)
        {
            using (var context = _contextFactory.Invoke())
            {

                Guid id = await GetUsersIdAsync(credentials.Email);
                if (id != default)
                {
                    return false;
                }

                id = Guid.NewGuid();
                //user id is salt for hash function
                var user = new User() { Id = id, Name = credentials.Name, Email = credentials.Email, Password = _hash.Hash(id.ToByteArray(), credentials.Password) };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            return true;
        }

        public async Task DeleteUserAsync(Guid id)
        {
            using var context = _contextFactory.Invoke();
            var user = await context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            using var context = _contextFactory.Invoke();
            return await context.Users.Where(u => u.Id == id).Select(s => new User() { Id = s.Id, Email = s.Email, Name = s.Name, Password = ""}).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserAsync(string email, string password)
        {
            using var context = _contextFactory.Invoke();
            return await context.Users.Where(u => u.Email == email && u.Password == password).Select(s => new User() { Id = s.Id, Email = s.Email, Name = s.Name, Password = "" }).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetUsersAsync()
        {
            using var context = _contextFactory.Invoke();
            return await context.Users.Select(s => new User() { Id = s.Id, Email = s.Email, Name = s.Name, Password = "" }).ToListAsync();
        }

        public async Task<Guid> GetUsersIdAsync(string email)
        {
            using var context = _contextFactory.Invoke();
            return await context.Users.Where(x => x.Email == email).Select(s => s.Id).FirstOrDefaultAsync();
        }
    }
}
