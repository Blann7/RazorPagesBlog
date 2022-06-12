using Microsoft.EntityFrameworkCore;

namespace BlogPhone.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<ArticleBlog> ArticleBlogs { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<AboutUsInfo> AboutUsPage { get; set; } = null!;
        public DbSet<Referral> Referrals { get; set; } = null!;

        public ApplicationContext() { }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=31.31.196.234;Database=u1690754_bp;User Id=u1690754_admin;Password=LenaGolovach777");
        }
    }
}
