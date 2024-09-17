using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Domain.DTOs.UserDTOs;
using Domain.Models;
using Domain.Responses;
using Infrastructure.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.UserService
{
    public class UserService(DataContext context):IUserService
    {
         public async Task<Response<List<GetUserDto>>> GetUsers()
    {
        try
        {
            var users = await context.Users.Select(x => new GetUserDto()
            {
                Username = x.UserName,
                Id = x.Id,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber
            }).ToListAsync();

            return new Response<List<GetUserDto>>(users);
        }
        catch (Exception e)
        {
            return new Response<List<GetUserDto>>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    public async Task<Response<GetUserDto>> GetUserById(int id)
    {
         try
        {
            var existingUser = await context.Users.Where(x => x.Id == id).Select(x =>
                new GetUserDto()
                {
                Username = x.UserName,
                Id = x.Id,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber
                }).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                return new Response<GetUserDto>(HttpStatusCode.BadRequest, "User not found");
            }
            return new Response<GetUserDto>(existingUser);
        }
        catch (Exception e)
        {
            return new Response<GetUserDto>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

         public async Task<Response<string>> UpdateUserAsync(UpdateUserDto updateUser)
    {
        try
        {

            var existing = await context.Users.Where(x => x.Id == updateUser.Id)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(u => u.Email, updateUser.Email)
                    .SetProperty(u => u.PhoneNumber, updateUser.PhoneNumber)
                    .SetProperty(u => u.UserName, updateUser.Username)
                    .SetProperty(u => u.HashPassword, BCrypt.Net.BCrypt.HashPassword(updateUser.Password) )

                );
                return existing == 0
                ? new Response<string>(HttpStatusCode.BadRequest, "Invalid request ")
                : new Response<string>("Successfully updated ");

        }
        catch (Exception e)
        {
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }


    public async Task<Response<string>> AddUser(AddUserDto addUser)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x=>x.UserName==addUser.Username);
            if (user!=null)
            {
                return new Response<string>("User is already exist");
            }
            var newUser = new User()
            {
               UserName = addUser.Username,
               Email = addUser.Email,
               PhoneNumber = addUser.PhoneNumber,
               HashPassword = BCrypt.Net.BCrypt.HashPassword(addUser.Password),
               Role = "Admin"
            };
            var res = await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();
           
            return new Response<string>($"Successfully created a new product by id:{newUser.Id}");
        }
        catch (Exception e)
        {
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    public async Task<Response<bool>> DeleteUserAsync(int id)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return new Response<bool>(HttpStatusCode.NotFound, "User Not Found");

            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return new Response<bool>(HttpStatusCode.OK, "User deleted succesfuly");
        }
        catch (Exception ex)
        {
            return new Response<bool>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    }
}