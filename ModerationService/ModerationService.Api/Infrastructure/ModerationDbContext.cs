using Microsoft.EntityFrameworkCore;
using ModerationService.Api.Models;

namespace ModerationService.Api.Infrastructure
{
    public class ModerationDbContext : DbContext
    {
        public ModerationDbContext(DbContextOptions<ModerationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Report> Reports { get; set; }
        public DbSet<Sanction> Sanctions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Report");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("char(36)");
                entity.Property(e => e.ReportedUserId).IsRequired().HasColumnType("char(36)");

                entity.Property(e => e.ContentType)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.Reason);
                entity.Property(e => e.ReportedAt)
                    .HasColumnType("timestamptz")  // PostgreSQL tipo "timestamp with time zone"
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");


                entity.Property(e => e.Status)
                    .HasConversion<string>(); 
            });

            modelBuilder.Entity<Sanction>(entity =>
            {
                entity.ToTable("Sanction");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("char(36)");
                entity.Property(e => e.ReportId).HasColumnType("char(36)");
                entity.HasIndex(e => e.ReportId).IsUnique();
                entity.Property(e => e.UserId).IsRequired().HasColumnType("char(36)");

                entity.Property(e => e.Type)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate);
                entity.Property(e => e.Reason);

                entity.HasOne(e => e.Report)
                    .WithOne()
                    .HasForeignKey<Sanction>(s => s.ReportId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
