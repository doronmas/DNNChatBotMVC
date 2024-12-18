using Microsoft.EntityFrameworkCore;
using DNNChatBotMVC.Models;

namespace DNNChatBotMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ContentEmbedding> ContentEmbeddings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ContentEmbedding>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SourceType).IsRequired();
                entity.Property(e => e.Title).HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.Content).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Embedding).HasColumnType("varbinary(max)");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.LastUpdated).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
