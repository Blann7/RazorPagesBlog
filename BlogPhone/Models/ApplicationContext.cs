using Microsoft.EntityFrameworkCore;
using BlogPhone.Pages;

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
    }
}
