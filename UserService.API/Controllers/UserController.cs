using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs;
using UserService.Application.DTOs.Requests;


[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerDto)
    {
        var result = await _userService.RegisterAsync(registerDto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginDto)
    {
        var result = await _userService.LoginAsync(loginDto);
        return Ok(result);
    }
}