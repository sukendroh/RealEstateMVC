using Microsoft.EntityFrameworkCore;
using RealEstateMVC.Models;

namespace RealEstateMVC.Data
{    
    public class RealEstateContext : DbContext
    {
        public RealEstateContext(DbContextOptions<RealEstateContext> options) : base(options) { }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
    }
}
