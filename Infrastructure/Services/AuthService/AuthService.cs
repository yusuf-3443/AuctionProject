using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain.DTOs.AuthDTOs;
using Domain.Models;
using Domain.Responses;
using Infrastructure.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.AuthService;

 public class AuthService(DataContext context, IConfiguration configuration) : IAuthService
{
    #region Login

    public async Task<Response<string>> Login(LoginDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(x =>
            x.UserName == loginDto.UserName);
if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.HashPassword))
{
    return new Response<string>(HttpStatusCode.BadRequest, "Incorrect name or password");
}
return new Response<string>(await GenerateJwtToken(user));
            }

    #endregion

    #region GenerateJwtToken

    private async Task<string> GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.PhoneNumber),
        };
        
        //add roles
        var role = await context.Users.FirstOrDefaultAsync(x=>x.Id==user.Id);
            claims.Add(new Claim(ClaimTypes.Role, role!.Role));
        

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        var securityTokenHandler = new JwtSecurityTokenHandler();
        var tokenString = securityTokenHandler.WriteToken(token);
        return tokenString;
    }

    #endregion
}
