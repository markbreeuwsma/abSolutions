using Microsoft.EntityFrameworkCore;

namespace DatabaseModel
{
    public class DatabaseContext : DbContext
    {
        // Current method for creating database and context is done using code first approach,
        // a way of developing known as Domain Driven Development.

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
        // is done by the following call 'Add-Migration (migrationname)'. 
        // For example 'add-migration InitialCreate' and 'add-migration FeatureName(Update)'. 
        // DO NOT remove the last migration deleting the related cs file, because it will corrupt
        // the migrationset. Use the 'Remove-migration' command instead.
        // The 'update-database (migrationname)' will update the database definition (up or down
        // to a specific migrationname if passed). In this project this call is not needed, since
        // the startup.cs file is setup for automatic migration where needed.
        // The PM command prompt consists of a few more commands for migration back and forth and 
        // debugging purposes. Check online documentation if needed.

        // Connection string "Server=xxx\\SQLExpress;Database=abSolutions;Trusted_Connection=True;"
        // works when replacing xxx with 'localhost' or 'HOME' (pc-name), NOT '(localdb)' !!!

        // For additional information check out http://www.entityframeworktutorial.net

        // Lazy loading was not supported in EF Core 2.0 like in EF 6, but added in 2.1 all-be-it 
        // by the use of a new NuGet package Microsoft.EntityFrameworkCore.Proxies and setting an
        // additional option 'UseLazyLoading' when setting the context up in startup.cs to activate.
        // However, then all entity types need to be public, unsealed and virtual, even if you don't
        // want lazy loading, so not optimal yet.


        // Passing on the DbContextOptions allows for dependency injection which helps with setting up 
        // tests and allows for the scope of the DbContext to be regulated by the web application.
        // Injecting the desired options in a web application is done in startup.cs of the web project.
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        // This function could be used to configure the database connection and migration stuff
        // when creating a new context, however in this solution this is done in the 
        // startup.cs of the WebApplication_MVC project
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //}

        // The database model derived from the classes can be influenced by adding either data 
        // annotation attributes in the class definition or adding additional configuration in
        // this function. Not all settings can be done using data annotations ! See blog.cs for 
        // the idea for data annotations. Fluent API overrules data annotations.
        // Property names can be entered as strings, but this disables the compile-time check for
        // correct fields and typo's, so using the lambda expressions is best practice.
        //
        // See http://www.entityframeworktutorial.net/efcore/fluent-api-in-entity-framework-core.aspx for a complete list
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().HasIndex(x => new { x.Created, x.BlogId })                
                                       .IsUnique();

            modelBuilder.Entity<Blog>().Property(x => x.Description)
                                       .HasColumnName("Description")
                                       .HasColumnType("nvarchar")
                                       .HasMaxLength(80)
                                       .IsRequired();

            // This one-to-many connection is already configured due to the EF naming conventions, however this is how Fluent API would do it
            // Map a one-to-many connection where a post links to one specific blog and a blog links to many posts
            modelBuilder.Entity<Post>()                         // working with the Post class
                        .HasOne<Blog>(post => post.Blog)        // where the Post class has one Blog property
                        .WithMany(blog => blog.Posts)           // and the Blog class is with many Posts
                        .HasForeignKey(post => post.BlogId);    // link them together on this property with a foreign key
            // Map the same one-to-many connection from the other way around, redundant here where it not for the extra OnDelete clause
            modelBuilder.Entity<Blog>()                         // working with the Blog class
                        .HasMany<Post>(blog => blog.Posts)      // where the Blog class is with many Posts
                        .WithOne(post => post.Blog)             // and the Post class has one Blog property
                        .HasForeignKey(post => post.BlogId)     // link them together on this property with a foreign key
                        .OnDelete(DeleteBehavior.Cascade);      // what action to take when Blog is deleted (Cascade will delete all linked Posts (in the context?))

            // Mapping one-to-one connections works simular but with HasOne WithOne clauses

            // Mapping many-to-many connections can only be done by adding a new entity class which holds properties for
            // the keys of both classes to link together, sets up a composite primairy key for the new class and then 
            // setup two one-to-many mappings from that new entity class to the original classes (check documentation)

            modelBuilder.Entity<Address>()
                        .Property(x => x.CountryId)
                        .HasMaxLength(2);

            modelBuilder.Entity<Contact>()
                        .Property(x => x.Created)
                        .ValueGeneratedOnAdd();
            modelBuilder.Entity<Contact>()
                        .Property(x => x.LastUpdated)
                        .ValueGeneratedOnUpdate();
            modelBuilder.Entity<Contact>()
                        .Property(x => x.RowVersion)
                        .IsRowVersion();

            modelBuilder.Entity<Address>()
                        .HasOne<Contact>(x => x.VisitingAddressContact)
                        .WithOne(x => x.VisitingAddress)
                        .HasForeignKey<Contact>(x => x.VisitingAddressId)
                        .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Address>()
                        .HasOne<Contact>(x => x.PostalAddressContact)
                        .WithOne(x => x.PostalAddress)
                        .HasForeignKey<Contact>(x => x.PostalAddressId)
                        .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Contact>()
                        .HasMany<Address>(x => x.DeliveryAddresses)
                        .WithOne(x => x.DeliveryAddressContact)
                        .HasForeignKey(x => x.DeliveryAddressContactId)
                        .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Contact>()
            //            .OwnsOne<Address>(x => x.VisitingAddress);
            //modelBuilder.Entity<Contact>()
            //            .OwnsOne<Address>(x => x.PostalAddress);

            modelBuilder.Entity<Country>()
                        .Property(x => x.CountryId)
                        .ValueGeneratedNever()
                        .HasMaxLength(2);

            modelBuilder.Entity<CountryDescription>()
                        .Property(x => x.CountryId)
                        .HasMaxLength(2)
                        .IsRequired();
            modelBuilder.Entity<CountryDescription>()
                        .Property(x => x.LanguageId)
                        .HasMaxLength(3)
                        .IsRequired();
            modelBuilder.Entity<CountryDescription>()
                        .Property(x => x.Description)
                        .HasMaxLength(80)
                        .IsRequired();
            modelBuilder.Entity<CountryDescription>()
                        .HasKey(x => new { x.CountryId, x.LanguageId });
            modelBuilder.Entity<CountryDescription>()
                        .HasIndex(x => new { x.LanguageId, x.CountryId })
                        .IsUnique();
            modelBuilder.Entity<CountryDescription>()
                        .HasOne<Country>(x => x.Country)
                        .WithMany(x => x.Descriptions)
                        .HasForeignKey(x => x.CountryId)
                        .OnDelete(DeleteBehavior.Cascade);
        }

        // This function could be used to extend the implementation of SaveChanges to add additional
        // processing or database requirements, e.g. generic exception logging or additional data validations
        //public override int SaveChanges()
        //{
        //    foreach (var entry in ChangeTracker.Entries <...> ())
        //    {
        //        if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
        //        {
        //            // Possibly check for null or if it's changed at all
        //            entry.Entity.Name = entry.Entity.Name.ToUpper();
        //        }
        //    }
        //    return base.SaveChanges();
        //}

        // Entities accessable and trackable by the context
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Post> Posts { get; set; }

        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CountryDescription> CountryDescriptions { get; set; }

    }
}
