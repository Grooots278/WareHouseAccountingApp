using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServerPartProgram.Models.User_Models;

namespace ServerPartProgram.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PasswordHistory> PasswordHistories { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.HasIndex(u => u.Username)
                      .IsUnique();

                entity.HasIndex(u => u.CreatedAt);

                entity.HasMany(u => u.RefreshTokens)
                      .WithOne(rt => rt.User)
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.PasswordHistories)
                      .WithOne(ph => ph.User)
                      .HasForeignKey(ph => ph.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(254);

                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(50);
            });

            // Конфигурация RefreshToken
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(rt => rt.Token)
                      .IsUnique();

                entity.HasIndex(rt => rt.UserId);

                entity.HasIndex(rt => rt.ExpiresAt);

                entity.Property(rt => rt.Token)
                      .IsRequired()
                      .HasMaxLength(500);

                // Каскадное удаление при удалении пользователя
                entity.HasOne(rt => rt.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Конфигурация PasswordHistory
            modelBuilder.Entity<PasswordHistory>(entity =>
            {
                entity.HasIndex(ph => new { ph.UserId, ph.CreatedAt });

                entity.Property(ph => ph.PasswordHas)
                      .IsRequired()
                      .HasMaxLength(500);
            });

            // Конфигурация AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasIndex(al => al.UserId);

                entity.HasIndex(al => al.CreatedAt);

                entity.HasIndex(al => al.Action);

                entity.Property(al => al.Action)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(al => al.IpAddress)
                      .HasMaxLength(45);
            });

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) ||
                        property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is User &&
                           (e.State == EntityState.Modified || e.State == EntityState.Added));

            foreach (var entityEntry in entries)
            {
                ((User)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
