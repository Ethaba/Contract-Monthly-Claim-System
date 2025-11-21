using CMCS.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Claim> Claims { get; set; } = null!;
        public DbSet<ClaimItem> ClaimItems { get; set; } = null!;
        public DbSet<SupportingDocument> SupportingDocuments { get; set; } = null!;
        public DbSet<Approval> Approvals { get; set; } = null!;
    }
}
