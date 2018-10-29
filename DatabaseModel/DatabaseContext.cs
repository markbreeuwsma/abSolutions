using Microsoft.EntityFrameworkCore;

namespace DatabaseModel
{
    public class DatabaseContext : DbContext
    {
        // Passing on the DbContextOptions allows for dependency injection which helps with setting up 
        // tests and allows for the scope of the DbContext to be regulated by the web application.
        // Injecting the desired options in a web application is done in startup.cs of the web project.
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}
