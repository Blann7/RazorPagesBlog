using Microsoft.EntityFrameworkCore;

namespace BlogPhone.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<ArticleBlog> ArticleBlogs { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=database;Trusted_Connection=True");
            // For RegRu - Server=localhost;Database=u1690754_main;User Id=u1690754_user;Password=LenaGolovach777;
            // For developing - Server=(localdb)\\mssqllocaldb;Database=database;Trusted_Connection=True
        }
    }
}
