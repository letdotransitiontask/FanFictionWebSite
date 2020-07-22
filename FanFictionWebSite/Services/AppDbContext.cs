using FanFictionWebSite.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Services
{
    public class AppDbContext : IdentityDbContext<User>
    {

        public DbSet<FanFiction> FanFictions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<FanFictionBadge> FanFictionBadges { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<FanFictionBadge>().HasOne(x => x.Badge).WithMany(x => (IEnumerable<FanFictionBadge>)x.FanFictionBadges).HasForeignKey(x => x.BadgeId);
            builder.Entity<FanFictionBadge>().HasOne(x => x.FanFiction).WithMany(x => (IEnumerable<FanFictionBadge>)x.FanFictionBadges).HasForeignKey(x => x.FanFictionId);
            builder.Entity<Category>().HasData(
                    new Category[]
                    {
                            new Category(){Name = "Category 1", Id = 1},
                            new Category(){Name = "Category 2", Id = 2},
                            new Category(){Name = "Category 3", Id = 3},
                            new Category(){Name = "Category 4", Id = 4},
                            new Category(){Name = "Category 5", Id = 5},
            });
        }
    }
}
