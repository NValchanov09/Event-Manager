using Microsoft.AspNetCore.Identity;
using EventManagerBackend;
using EventManagerBackend.Extensions;
using EventManagerBackend.Models;
using EventManagerBackend.Seeders;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppServices()
    .AddAppDbContext(builder.Configuration)
    .AddAppIdentity()
    .AddCorsSupport()
    .AddAppSwagger()
	.SetupMailer(builder.Configuration);

var app = builder.Build();

app.ApplyMigrations();

// IN DEVELOPMENT STUFF HERE
if (app.Environment.IsDevelopment())
{
    //Swagger in DEV
    app.ConfigureSwagger();
    await app.ConfigureDemoSeederAsync();
}

await app.ConfigureSeederAsync();

// CORS support
app.UseCors("AllowAll");

// Use authentication & authorization
app.MapIdentityApi<User>();

// API endpoints
app.MapEventEndpoints();

app.Run();