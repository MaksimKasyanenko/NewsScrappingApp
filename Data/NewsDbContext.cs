using System;
using Microsoft.EntityFrameworkCore;

namespace NewsParsingApp.Data
{
    public class NewsDbContext : DbContext
    {
        public NewsDbContext(string connectionString)
        {
            this._connectionString = connectionString;
        }
        
        private readonly string _connectionString;

        public DbSet<News> News { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(this._connectionString);
        }
    }
}