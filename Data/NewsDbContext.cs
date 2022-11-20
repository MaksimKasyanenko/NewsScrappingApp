using System;
using Microsoft.EntityFrameworkCore;

namespace NewsParsingApp.Data
{
    public class NewsDbContext : DbContext
    {
        public NewsDbContext()
        {
            var path = Environment.CurrentDirectory;
            DbPath = System.IO.Path.Join(path, "news.db");
        }
        
        private string DbPath {get;}

        public DbSet<News> News { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath};");
        }
    }
}