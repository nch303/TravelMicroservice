using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Requests;
using UserService.Application.IServices;
using UserService.Domain.Entities;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetById(id);
            var userResponse = _mapper.Map<UpdateUserRequest>(user);
            return Ok(userResponse);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            var userResponses = _mapper.Map<List<UpdateUserRequest>>(users);
            return Ok(userResponses);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            var updatedUser = await _userService.UpdateProfile(user);
            var userResponse = _mapper.Map<UpdateUserRequest>(updatedUser);
            return Ok(userResponse);
        }
    }
}
