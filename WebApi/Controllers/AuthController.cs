using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Responses;
using Domain.DTOs.AuthDTOs;
using Infrastructure.Services.AuthService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace WebApi.Controllers
{ [ApiController]
  [Route("api/[controller]")]
  public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response = await authService.Login(loginDto);
        return StatusCode(response.StatusCode, response);
    }
    }
}