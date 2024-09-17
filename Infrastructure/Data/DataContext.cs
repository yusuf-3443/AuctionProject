using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Data;

      public class DataContext:DbContext
    {
         public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}
