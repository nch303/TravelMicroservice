using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.IServices;
using UserService.Domain.Entities;
using UserService.Domain.IRepositories;

namespace UserService.Application.Services
{
    public class UsersService: IUserService
    {
        private readonly IUserRepository _userRepository;

        public UsersService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public Task<List<User>> GetAll()
        {
            var users = _userRepository.GetAllAsync();
            if(users == null || users.Result.Count == 0)
            {
                throw new Exception("No users found");
            }
            return users;
        }

        public Task<User?> GetById(Guid id)
        {
            var user = _userRepository.GetByIdAsync(id);
            if(user == null || user.Result == null)
            {
                throw new Exception("User not found");
            }
            return user;
        }

        public async Task<User> UpdateProfile(User user)
        {
            var existedUser = await _userRepository.GetByIdAsync(user.Id);
            if(existedUser == null || existedUser == null)
            {
                throw new Exception("User not found");
            }

            // Update
            existedUser.Name = user.Name;
            existedUser.Address = user.Address;
            existedUser.PhoneNumber = user.PhoneNumber;
            existedUser.Gender = user.Gender;
            existedUser.DateOfBirth = user.DateOfBirth;
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                existedUser.AvatarUrl = user.AvatarUrl;
            }

            return await _userRepository.UpdateProfileAsync(existedUser);
        }

        public async Task<User> CreateProfile(User user)
        {
            var existedUser = _userRepository.GetByIdAsync(user.Id);
            if (existedUser != null && existedUser.Result != null)
            {
                throw new Exception("User already exists");
            }
            return await _userRepository.CreateProfileAsync(user);
        }
    }
}
