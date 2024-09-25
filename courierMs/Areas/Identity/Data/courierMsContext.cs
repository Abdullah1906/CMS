using courierMs.Areas.Identity.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using courierMs.DataModel;

namespace courierMs.Data
{
    public class courierMsContext : IdentityDbContext<ApplicationUser>
    {
        public courierMsContext(DbContextOptions<courierMsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        public DbSet<Customerinfo> Customerinfo { get; set; } = default!;
        public DbSet<Percelinfo> Percelinfo { get; set; } = default!;
        public DbSet<Lookup> Lookup { get; set; } = default!;




    }
}
