using Microsoft.EntityFrameworkCore;
using Shashlik.RC.Data.Entities;
// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Data
{
    public class RCDbContext : DbContext
    {
        public RCDbContext(DbContextOptions<RCDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.RegisterEntitiesFromAssembly<IEntity>(typeof(RCDbContext).Assembly);
            modelBuilder.Entity<Configs>().HasQueryFilter(r => !r.IsDeleted);
        }
    }
}
