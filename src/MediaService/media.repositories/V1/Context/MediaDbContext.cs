using media.models.V1.Db;
using Microsoft.EntityFrameworkCore;

namespace media.repositories.V1.Context;

public class MediaDbContext(DbContextOptions<MediaDbContext> _options) : DbContext(_options)
{
    public DbSet<Media> Media { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Media>(entity =>
        {
            entity.ToTable("media", "healthcare", t => t.HasComment("Stores media information"));
            entity.HasKey(e => e.MediaId);

            entity.Property(e => e.MediaId)
                .HasColumnName("media_id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.FileName)
                .HasColumnName("file_name")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.FileUrl)
                .HasColumnName("file_url")
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.UploadDate)
                .HasColumnName("upload_date")
                .IsRequired();

            entity.Property(e => e.ContentType)
                .HasColumnName("content_type")
                .HasMaxLength(50)
                .IsRequired();
        });
    }
}
