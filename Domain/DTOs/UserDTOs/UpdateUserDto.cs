using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.DTOs.UserDTOs
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
         public required string Username { get; set; } 
    public required string Email { get; set; } 
    public required string PhoneNumber { get; set; } 
    public required string Password { get; set; }
    }
}