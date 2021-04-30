using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shashlik.EfCore;
using Shashlik.RC.Data.Entities;

// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Data
{
    [AutoMigration]
    public class RCDbContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
    {
        public RCDbContext(DbContextOptions<RCDbContext> options) : base(options)
        {
        }

        public DbSet<Applications> Applications { get; set; }
        public DbSet<Environments> Environments { get; set; }
        public DbSet<ConfigurationFiles> Files { get; set; }
        public DbSet<Logs> Logs { get; set; }
        public DbSet<SignatureKeys> SignatureKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new Applications.Configs());
            builder.ApplyConfiguration(new Environments.Configs());
            builder.ApplyConfiguration(new ConfigurationFiles.Configs());
            builder.ApplyConfiguration(new Logs.Configs());
            builder.ApplyConfiguration(new SignatureKeys.Configs());
        }
    }
}