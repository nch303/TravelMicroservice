using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;

namespace UserService.Domain.IRepositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<List<User>> GetAllAsync();
        Task<User> UpdateProfileAsync(User user);
        Task<User> CreateProfileAsync(User user);
    }
}
