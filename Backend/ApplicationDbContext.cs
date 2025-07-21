using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventManagerBackend.Models;
using EventManagerBackend.Models.JSON;
using System.Text.Json;

namespace EventManagerBackend
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Event> Events { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                //Email should be unique
                entity.HasIndex(u => u.Email)
                .IsUnique();
            });
                

            modelBuilder.Entity<Event>(entity =>
            {
                //Json serialization for fields
                entity.Property(e => e.Fields)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<IList<Field>>(v!, new JsonSerializerOptions())
                );

                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                //Composite key for Submission
                entity.HasKey(s => new { s.EventId, s.UserId });

                //Foreign keys for Submission
                entity.HasOne(s => s.Event)
                .WithMany(e => e.Submissions)
                .HasForeignKey(s => s.EventId);

                entity.HasOne(s => s.User)
                .WithMany(u => u.Submissions)
                .HasForeignKey(s => s.UserId);

                //Json serialization for submissions
                entity.Property(e => e.Answers)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<IList<Answer>>(v!, new JsonSerializerOptions())
                );
            });
                
            base.OnModelCreating(modelBuilder);
        }
    }
}

