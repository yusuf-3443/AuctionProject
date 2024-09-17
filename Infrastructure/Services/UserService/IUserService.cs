using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.DTOs.UserDTOs;
using Domain.Responses;

namespace Infrastructure.Services.UserService
{
    public interface IUserService
    {
    Task<Response<List<GetUserDto>>> GetUsers();
    Task<Response<GetUserDto>> GetUserById(int id);
    Task<Response<string>> UpdateUserAsync(UpdateUserDto updateUser);
    Task<Response<string>> AddUser(AddUserDto addUser);
    Task<Response<bool>> DeleteUserAsync(int id);
    }

}