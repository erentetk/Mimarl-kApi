using Microsoft.EntityFrameworkCore;
using Mimarlik.Core.Entities;
using Mimarlik.Core.Enums;

namespace Mimarlik.Infrastructure.Data;

public class MimarlikDbContext : DbContext
{
    public MimarlikDbContext(DbContextOptions<MimarlikDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<ContentBlock> ContentBlocks { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Translation> Translations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Description).HasMaxLength(1000);
            
            // Self-referencing relationship for hierarchy
            entity.HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Project configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(300);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Client).HasMaxLength(200);
            entity.Property(e => e.Area).HasPrecision(18, 2);
            entity.Property(e => e.AreaUnit).HasMaxLength(10);
            entity.Property(e => e.MetaTitle).HasMaxLength(300);
            entity.Property(e => e.MetaDescription).HasMaxLength(500);
            entity.Property(e => e.MetaKeywords).HasMaxLength(500);
            
            entity.HasOne(e => e.Category)
                .WithMany(e => e.Projects)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Photo configuration
        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.OriginalFileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileSize).HasMaxLength(50);
            entity.Property(e => e.MimeType).HasMaxLength(100);
            entity.Property(e => e.AltText).HasMaxLength(200);
            entity.Property(e => e.Caption).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.SliderText).HasMaxLength(500);
            
            entity.HasOne(e => e.Project)
                .WithMany(e => e.Photos)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ContentBlock configuration
        modelBuilder.Entity<ContentBlock>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Properties).HasColumnType("nvarchar(max)");
            
            entity.HasOne(e => e.Project)
                .WithMany(e => e.ContentBlocks)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Language configuration
        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NativeName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Translation configuration
        modelBuilder.Entity<Translation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FieldName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).IsRequired();
            
            entity.HasOne(e => e.Language)
                .WithMany(e => e.Translations)
                .HasForeignKey(e => e.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Composite unique index for entity + field + language
            entity.HasIndex(e => new { e.EntityName, e.EntityId, e.FieldName, e.LanguageId })
                .IsUnique();
        });

        // Configure enums
        modelBuilder.Entity<Category>()
            .Property(e => e.Status)
            .HasConversion<int>();

        modelBuilder.Entity<Project>()
            .Property(e => e.Status)
            .HasConversion<int>();

        modelBuilder.Entity<Photo>()
            .Property(e => e.Status)
            .HasConversion<int>();

        modelBuilder.Entity<ContentBlock>()
            .Property(e => e.Status)
            .HasConversion<int>();

        modelBuilder.Entity<ContentBlock>()
            .Property(e => e.Type)
            .HasConversion<int>();

        modelBuilder.Entity<Language>()
            .Property(e => e.Status)
            .HasConversion<int>();

        // Set default values
        modelBuilder.Entity<Category>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Project>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Photo>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<ContentBlock>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Language>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Translation>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Seed default data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default languages
        modelBuilder.Entity<Language>().HasData(
            new Language
            {
                Id = 1,
                Code = "tr",
                Name = "Türkçe",
                NativeName = "Türkçe",
                IsDefault = true,
                Status = ContentStatus.Published,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Language
            {
                Id = 2,
                Code = "en",
                Name = "English",
                NativeName = "English",
                IsDefault = false,
                Status = ContentStatus.Published,
                SortOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Language
            {
                Id = 3,
                Code = "de",
                Name = "Deutsch",
                NativeName = "Deutsch",
                IsDefault = false,
                Status = ContentStatus.Published,
                SortOrder = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // Seed a default category
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Title = "Mimari Projeler",
                Description = "Genel mimari projeler",
                Slug = "mimari-projeler",
                Status = ContentStatus.Published,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                e.State == EntityState.Added ||
                e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;
            
            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            
            entity.UpdatedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}