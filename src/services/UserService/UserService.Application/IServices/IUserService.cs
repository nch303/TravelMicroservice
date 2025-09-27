using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;

namespace UserService.Application.IServices
{
    public interface IUserService
    {
        Task<User?> GetById(Guid id);
        Task<List<User>> GetAll();
        Task<User> UpdateProfile(User user);
        Task<User> CreateProfile(User user);
    }
}
