using Microsoft.EntityFrameworkCore;
using SportCenter.Models;

namespace SportCenter.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<ActiveSession> ActiveSessions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Equipment> Equipment { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=sportcenter.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>()
                .HasOne(m => m.Trainer)
                .WithMany(t => t.Members)
                .HasForeignKey(m => m.TrainerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ActiveSession>()
                .HasOne(s => s.Member)
                .WithMany(m => m.ActiveSessions)
                .HasForeignKey(s => s.MemberId);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Member)
                .WithMany(m => m.Payments)
                .HasForeignKey(p => p.MemberId);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Member)
                .WithMany(m => m.Sessions)
                .HasForeignKey(s => s.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
