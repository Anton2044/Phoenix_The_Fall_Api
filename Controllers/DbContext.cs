using Microsoft.EntityFrameworkCore;
using Phoenix_The_Fall_Web_application.Controllers;
using Phoenix_The_Fall_Web_application.Models;

namespace Phoenix_The_Fall_Web_application.DataBase
{
    public class YourDbContext : DbContext
    {
        public YourDbContext(DbContextOptions<YourDbContext> options) : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<PlayerWeapon> PlayerWeapons { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<PlayerClass> PlayerClasses { get; set; }
        public DbSet<ClassLoadout> ClassLoadouts { get; set; }
        public DbSet<Granate> Granates { get; set; } // Новая таблица
        public DbSet<PlayerGranate> PlayerGranates { get; set; } // Новая таблица
        public DbSet<Match> Matches { get; set; }
        public DbSet<Map> Maps { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Granate>().HasKey(g => g.Id);
            modelBuilder.Entity<PlayerGranate>().HasKey(pg => pg.Id);

            modelBuilder.Entity<ClassLoadout>()
            .HasOne(cl => cl.Granate)
            .WithMany()
            .HasForeignKey(cl => cl.GranateId)
            .OnDelete(DeleteBehavior.SetNull); // Или DeleteBehavior.Cascade, в зависимости от логики

            // Связь PlayerGranate с Granates
            modelBuilder.Entity<PlayerGranate>()
                .HasOne(pg => pg.Granate)
                .WithMany()
                .HasForeignKey(pg => pg.WeaponId)
                .OnDelete(DeleteBehavior.Cascade);

            //PlayerWeapons: Настройка связей
            modelBuilder.Entity<PlayerWeapon>()
                           .HasOne(pw => pw.Player)
                           .WithMany(p => p.PlayerWeapons)
                           .HasForeignKey(pw => pw.PlayerId);

            modelBuilder.Entity<PlayerWeapon>()
                .HasOne(pw => pw.Weapon)
                .WithMany()
                .HasForeignKey(pw => pw.WeaponId);

            modelBuilder.Entity<PlayerClass>()
                .HasOne(pc => pc.Class)
                .WithMany()
                .HasForeignKey(pc => pc.ClassId);

            modelBuilder.Entity<ClassLoadout>()
                .HasKey(cl => cl.PlayerClassId);  // Устанавливаем PlayerClassId как первичный ключ


            modelBuilder.Entity<ClassLoadout>()
                .HasOne(cl => cl.MainWeapon)
                .WithMany()
                .HasForeignKey(cl => cl.MainWeaponId);

            modelBuilder.Entity<ClassLoadout>()
                .HasOne(cl => cl.SecondaryWeapon)
                .WithMany()
                .HasForeignKey(cl => cl.SecondaryWeaponId);

            modelBuilder.Entity<ClassLoadout>()
                .HasOne(cl => cl.Gadget)
                .WithMany()
                .HasForeignKey(cl => cl.GadgetId);
        }
    }
}
