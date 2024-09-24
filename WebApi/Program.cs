using Infrastructure.Services.Data;
using Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using WebApp.ExtensionMethods.AuthConfigurations;
using WebApp.ExtensionMethods.RegisterService;
using WebApp.ExtensionMethods.SwaggerConfigurations;
using Domain.DTOs.EmailDTOs;

var builder = WebApplication.CreateBuilder(args);

var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig!);

// connection to database && dependency injection
builder.Services.AddRegisterService(builder.Configuration);


// register swagger configuration
builder.Services.SwaggerService();


// authentications service
builder.Services.AddAuthConfigureService(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();


// update database
try
{
    var serviceProvider = app.Services.CreateScope().ServiceProvider;
    var dataContext = serviceProvider.GetRequiredService<DataContext>();
    await dataContext.Database.MigrateAsync();

    //seed data
    var seeder = serviceProvider.GetRequiredService<Seeder>();
    await seeder.SeedUser();
}
catch (Exception)
{
    // ignored
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();