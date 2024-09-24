using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain.DTOs.AuthDTOs;
using Domain.DTOs.EmailDTOs;
using Domain.Models;
using Domain.Responses;
using Infrastructure.Services.Data;
using Infrastructure.Services.EmailService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.AuthService;

 public class AuthService(DataContext context, IConfiguration configuration,IEmailSender emailSender) : IAuthService
{
    #region Login

    public async Task<Response<string>> Login(LoginDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(x =>
            x.Email == loginDto.Email);
if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.HashPassword))
{
    return new Response<string>(HttpStatusCode.BadRequest, "Incorrect name or password");
}

 var random = new Random();

            user.Code = random.Next(1000, 10000).ToString();
            user.CodeTime = DateTime.UtcNow.AddMinutes(5);

            await context.SaveChangesAsync();

await emailSender.SendEmail(new EmailMessageDto(new[] {loginDto.Email}, "authentification code", $"<h1>{user.Code}</h1>"), MimeKit.Text.TextFormat.Html);
return new Response<string>("Check your email");
// return new Response<string>(await GenerateJwtToken(user));
            }

    #endregion

    #region VerifyCode
    
   public async Task<Response<string>> VerifyCode(string codecha)
{
    var user = await context.Users.FirstOrDefaultAsync(u => u.Code == codecha);

    if (user == null)
    {
        return new Response<string>(HttpStatusCode.BadRequest, "Invalid code.");
    }

    if (user.CodeTime < DateTime.UtcNow)
    {
        return new Response<string>(HttpStatusCode.BadRequest, "Code has expired.");
    }

    var token = await GenerateJwtToken(user);

    user.Code = null;
    await context.SaveChangesAsync();

    return new Response<string>(token);
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
