using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projects_Workshop.Models.Entities;

namespace Projects_Workshop.Models
{
    public class EFDBContext : DbContext
    {
        public EFDBContext(DbContextOptions<EFDBContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Teams);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Team)
                .WithMany(t => t.Members);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Project)
                .WithMany(p => p.Participants);
        }
    }
}
