using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {}

    public DbSet<Article> Articles { get; set; }
    // Add other entities here
}

public class Article
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}