﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace EventManagerBackend.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IHost app)
        {
            var runMigrations = Environment.GetEnvironmentVariable("RUN_MIGRATIONS");

            if (runMigrations != "true")
                return;

            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var retryPolicy = Policy
                .Handle<SqlException>()
                .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(5));

            retryPolicy.Execute(dbContext.Database.Migrate);
        }
    }
}
