using System;
using Microsoft.EntityFrameworkCore;

namespace NewsParsingApp.Data
{
    public class NewsDbContext : DbContext
    {
        public NewsDbContext()
        {
            this._connectionString = "Data Source=news.db;";
        }
        
        private readonly string _connectionString;

        public DbSet<News> News { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(this._connectionString);
        }
    }
}