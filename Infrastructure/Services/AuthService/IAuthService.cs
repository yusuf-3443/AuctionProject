using Domain.DTOs.AuthDTOs;
using Domain.Responses;

namespace Infrastructure.Services.AuthService;

public interface IAuthService
{
    public Task<Response<string>> Login(LoginDto loginDto);

}
