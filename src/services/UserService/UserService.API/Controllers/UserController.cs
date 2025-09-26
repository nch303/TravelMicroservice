﻿
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateUserRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Invalid token: User ID not found" });
                }

                string? avatarUrl = null;

                try
                {
                    if (request.AvatarUrl != null && request.AvatarUrl.Length > 0)
                    {
                        using var stream = request.AvatarUrl.OpenReadStream();
                        avatarUrl = await _cloudinaryService.UploadImageAsync(stream, request.AvatarUrl.FileName);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"File upload failed: {ex.Message}");
                }

                var user = _mapper.Map<User>(request);
                user.AvatarUrl = avatarUrl!;
                user.Id = userId; // Ensure the user ID is set from the token
                var updatedUser = await _userService.UpdateProfile(user);
                var userResponse = _mapper.Map<UserResponse>(updatedUser);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest request)
        {
            try
            {
                var user = _mapper.Map<User>(request);
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
