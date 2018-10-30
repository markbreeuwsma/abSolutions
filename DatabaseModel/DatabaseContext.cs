using Microsoft.EntityFrameworkCore;

namespace DatabaseModel
{
    public class DatabaseContext : DbContext
    {
        // Current method for creating database and context is done using code first approach

        // Creating migrations (code to create SQL statements to fit the model changes) is done
        // by using the Package Manager Console command line.
        // For the setup where the context definition is in one project and the connectionstring  
        // in another project (WebApplication_MVC), the startup project in VS needs to be set to 
        // WebApplication_MVC, but the PM console project be set up to this project (DatabaseModel) 
        // to add the migration rules there. 
        // For the PM console to work correctly add NuGet package Microsoft.EntityFrameworkCore.Tools
        // to the project. For the migration generated code to compile, it also needs NuGet packages
        // Microsoft.EntityFrameworkCore.Relational and Microsoft.EntityFrameworkCore.SqlServer.
        // Creating the initial definition for a new database and future migrations for model changes
        // is done by the following call:
        //  - Add-Migration (migrationname) 
        // for example 'add-migration InitialCreate' and 'add-migration FeatureName(Update)'. The tools 
        // consists of more commands for migration back and forth and debugging purposes. It is 
        // possible to update the database directly from the command 'update-database', but here
        // the migration is added to the startup.cs file for automatic migration.

        // Connection string "Server=xxx\\SQLExpress;Database=abSolutions;Trusted_Connection=True;"
        // works when replacing xxx with 'localhost' or 'HOME' (pc-name), NOT '(localdb)' !!!

        // For additional information check out http://www.entityframeworktutorial.net

        // Passing on the DbContextOptions allows for dependency injection which helps with setting up 
        // tests and allows for the scope of the DbContext to be regulated by the web application.
        // Injecting the desired options in a web application is done in startup.cs of the web project.
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            //this.Database.Migrate();
            //Database.SetInitializer<DatabaseContext>(new CreateDatabaseIfNotExists<DatabaseContext>());

            //Database.SetInitializer<SchoolDBContext>(new DropCreateDatabaseIfModelChanges<SchoolDBContext>());
            //Database.SetInitializer<SchoolDBContext>(new DropCreateDatabaseAlways<SchoolDBContext>());
            //Database.SetInitializer<SchoolDBContext>(new SchoolDBInitializer());

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }


        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
    }
}
