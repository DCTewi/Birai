using Microsoft.EntityFrameworkCore;

namespace Birai.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<VideoInfo> VideoInfos { get; set; }
        public DbSet<WatchedVideo> WatchedVideo { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
