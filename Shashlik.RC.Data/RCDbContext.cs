using Microsoft.EntityFrameworkCore;
using Shashlik.EfCore;
using Shashlik.RC.Data.Entities;

// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Data
{
    [AutoMigration]
    public class RCDbContext : ShashlikDbContext<RCDbContext>
    {
        public RCDbContext(DbContextOptions<RCDbContext> options) : base(options)
        {
        }
    }
}