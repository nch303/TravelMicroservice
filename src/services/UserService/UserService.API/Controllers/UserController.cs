
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Requests;
using UserService.Application.DTOs.Responses;
using UserService.Application.IServices;
using UserService.Domain.Entities;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _userService = userService;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var user = await _userService.GetById(id);
                var userResponse = _mapper.Map<UserResponse>(user);
                return Ok(userResponse);
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAll();
                var userResponses = _mapper.Map<List<UserResponse>>(users);
                return Ok(userResponses);
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request)
        {
            try 
            {
                var user = _mapper.Map<User>(request);
                var updatedUser = await _userService.UpdateProfile(user);
                var userResponse = _mapper.Map<UserResponse>(updatedUser);
                return Ok(userResponse);
            }
            catch
            {
                return BadRequest(new { message = "Update profile failed" });
            }
            
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProfile([FromForm] CreateProfileRequest request)
        {
            try 
            {
                string? avatarUrl = null;

                try
                {
                    if (request.AvatarUrl != null && request.AvatarUrl.Length > 0)
                    {
                        using var stream = request.AvatarUrl.OpenReadStream();
                        avatarUrl = await _cloudinaryService.UploadImageAsync(stream, request.AvatarUrl.FileName);
                    }
                }
                catch(Exception ex)
                {
                    return BadRequest($"File upload failed: {ex.Message}");
                }

                var user = _mapper.Map<User>(request);
                user.AvatarUrl = avatarUrl!;
                var createdUser = await _userService.CreateProfile(user);
                var userResponse = _mapper.Map<UserResponse>(createdUser);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }         
        }
    }
}
