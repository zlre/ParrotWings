namespace ParrotWingsAPI
{
    using ParrotWingsData;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUsersService
    {
        Task<User> GetUserAsync(Guid id);

        Task DeleteUserAsync(Guid id);

        Task<List<User>> GetUsersAsync();

        Task<User> GetUserAsync(string email, string password);

        Task<bool> AddUserAsync(RegisterCredentials credentials);

        Task<Guid> GetUsersIdAsync(string email);
    }
}
