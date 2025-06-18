using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VKRServer.DataBase
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<UserData> UserData { get; set; }
        public DbSet<ModerData> ModerData { get; set; }
        public DbSet<AdminData> AdminData { get; set; }
        public DbSet<TimeTable> TimeTable { get; set; }
        public DbSet<MarkTable> MarkTable { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(z => z.ID);

                entity.Property(z => z.ID)
                      .ValueGeneratedNever();

                entity.Property(z => z.Role)
                      .IsRequired()
                      .HasConversion<string>();

                entity.Property(z => z.Login)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(z => z.Password)
                      .IsRequired()
                      .HasMaxLength(60);

                entity.HasIndex(z => z.Login)
                      .IsUnique();
            });

            modelBuilder.Entity<UserData>(entity =>
            {
                BaseData(entity);
                entity.ToTable("UserData");

                entity.Property(z => z.Groop)
                      .IsRequired()
                      .HasMaxLength(10);

                entity.HasOne(z => z.User)
                      .WithOne(z => z.UserData)
                      .HasForeignKey<UserData>(z => z.ID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ModerData>(entity =>
            {
                BaseData(entity);
                entity.ToTable("ModerData");

                entity.Property(z => z.Key)
                      .HasColumnType("numeric(80, 0)");

                entity.HasOne(z => z.User)
                      .WithOne(z => z.ModerData)
                      .HasForeignKey<ModerData>(z => z.ID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AdminData>(entity =>
            {
                BaseData(entity);
                entity.ToTable("AdminData");

                entity.Property(z => z.Department)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.HasOne(z => z.User)
                      .WithOne(z => z.AdminData)
                      .HasForeignKey<AdminData>(z => z.ID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TimeTable>(entity =>
            {
                entity.ToTable("TimeTable");

                entity.HasKey(z => z.ID);

                entity.Property(z => z.ID)
                      .ValueGeneratedOnAdd();

                entity.Property(z => z.Groop)
                      .IsRequired()
                      .HasMaxLength(10);

                entity.Property(z => z.Name)
                      .IsRequired();

                entity.Property(z => z.Audience)
                      .IsRequired()
                      .HasMaxLength(10);

                entity.HasIndex(z => new { z.Groop, z.DayOfWeek, z.N, z.ModerID })
                      .IsUnique();

                entity.HasIndex(z => new { z.ModerID, z.DayOfWeek, z.N, z.Groop})
                      .IsUnique();
            });

            modelBuilder.Entity<MarkTable>(entity =>
            {
                entity.ToTable("MarkTable");

                entity.HasKey(z => z.ID);

                entity.Property(e => e.Mark)
                      .HasDefaultValue(0);

                entity.HasIndex(z => z.ID)
                      .IsUnique();

                entity.HasOne(z => z.UserData)
                      .WithOne(z => z.MarkTable)
                      .HasForeignKey<MarkTable>(z => z.ID)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void BaseData<T>(EntityTypeBuilder<T> builder) where T : BaseData
        {
               builder.HasKey(z => z.ID);

               builder.Property(z => z.FirstName)
                      .IsRequired()
                      .HasMaxLength(50);

               builder.Property(z => z.LastName)
                      .IsRequired()
                      .HasMaxLength(50);

               builder.Property(z => z.MiddleName)
                      .HasMaxLength(50);
        }
    }
}
//cd C:\Users\T0dS1K\Desktop\VKRServer\VKRServer; dotnet ef migrations add VKRServer
