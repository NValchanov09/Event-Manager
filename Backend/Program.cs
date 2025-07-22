using EventManagerBackend;
using EventManagerBackend.Extensions;
using EventManagerBackend.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppServices(builder.Configuration)
    .AddAppDbContext(builder.Configuration)
    .AddAppIdentity()
    .AddCorsSupport()
    .AddAppSwagger();

var app = builder.Build();

app.ApplyMigrations();

// IN DEVELOPMENT STUFF HERE
if (app.Environment.IsDevelopment())
{
    //Swagger in DEV
    app.ConfigureSwagger();

    using (var scope = app.Services.CreateScope())
    {
        var submissionService = scope.ServiceProvider.GetRequiredService<ISubmissionService>();
        await app.ConfigureDemoSeederAsync(submissionService);
    }
}

await app.ConfigureSeederAsync();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// CORS support
app.UseCors("AllowAll");

// Use authentication & authorization
app.MapIdentityApi<User>();

// API endpoints
app.MapEventEndpoints();

app.Run();