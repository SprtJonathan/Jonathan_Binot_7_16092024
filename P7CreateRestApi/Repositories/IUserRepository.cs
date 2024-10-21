﻿using P7CreateRestApi.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P7CreateRestApi.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<User> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
    }
}
