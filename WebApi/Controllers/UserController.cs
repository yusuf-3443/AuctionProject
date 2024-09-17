using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.DTOs.UserDTOs;
using Domain.Responses;
using Infrastructure.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
[Authorize( Roles = "SuperAdmin")]
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
        [HttpGet("user")]
    public async Task<Response<List<GetUserDto>>> GetUsers()
    {
       return await userService.GetUsers();
    }

    [HttpGet("{userId:int}")]
    public async Task<Response<GetUserDto>> GetCourseById(int userId)
        => await userService.GetUserById(userId);

    
    [HttpPost("users")]
    public async Task<IActionResult> AddUser([FromBody] AddUserDto addUser)
    {
        var res1 = await userService.AddUser(addUser);
        return StatusCode(res1.StatusCode, res1);
    }

    [HttpDelete("{userId:int}")]

    public async Task<IActionResult> DeleteUser(int userId)
    {
        var res1 = await userService.DeleteUserAsync(userId);
        return StatusCode(res1.StatusCode, res1);
    }


    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUser)
    {
        var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "sid")?.Value);
        var result = await userService.UpdateUserAsync(updateUser);
        return StatusCode(result.StatusCode, result);
    }
}
}