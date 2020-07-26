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
        public DbSet<Like> Likes { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Category>().HasData(
                    new Category[]
                    {
                            new Category(){Name = "Anime", Id = 1},
                            new Category(){Name = "Books", Id = 2},
                            new Category(){Name = "Cinema", Id = 3},
                            new Category(){Name = "Cartoons", Id = 4},
            });
        }
    }
}
