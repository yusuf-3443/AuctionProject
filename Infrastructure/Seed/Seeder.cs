using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seed
{
        public class Seeder(DataContext context)
{
    
    #region SeedUser

    public async Task SeedUser()
    {
        try
        {
            var existing = await context.Users.AnyAsync(x => x.UserName == "SuperAdmin");
            if (existing)
            {
                return;
            }

            var user = new User
            {
                Email = "polvonov.a@gmail.com",
                UserName = "SuperAdmin",
                HashPassword = BCrypt.Net.BCrypt.HashPassword("1234"),
                PhoneNumber = "+992007992757",
                Role = "SuperAdmin"
            };

            await context.AddAsync(user);
            await context.SaveChangesAsync();

            return;
        }
        catch (Exception e)
        {
        }
    }

    #endregion
    
}
    }
