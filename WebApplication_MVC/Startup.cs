using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using DatabaseModel;
using WebApplication_MVC.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;

namespace WebApplication_MVC
{
    // This class is used by the webhostbuilder and is accepted because it has the right signature of methods,
    // but any other class or name would do the same as long as the signatures are correct. So you can mock up
    // a whole new test startup class for use in unit testing.
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Setup dependency injection for localization by use of IStringLocalizer<T> for the controller classes
            // who can then retrieve information per localization PER CONTROLLER. The accessor will try to look up
            // a dictionairy key in the file <ResourcePath>/Controller.<ControllerName>.<language>.resx or subdirectory
            // <ResourcePath>/Controller/<ControllerName>.<language>.resx. If not found it will return the key as string,
            // which allows to develop the app initially without worrying to much about adding resources.
            //services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

            // Setup dependency injection of a repository, so that controles can make use of these repositories
            // by accepting them in the constructor, while the generic setup and control of the scope is done
            // by the container. In turn the repository will take the below setup service for a DbContext into 
            // their constructor and no longer in the controler itself adding to seperation of concerns.
            services.AddScoped<ICrudRepositoryAsync<FieldOfInterestView, string>, FieldOfInterestViewRepository>();

            // add a service to allow API calls to return XML formatted data instead of the default JSON. This is
            // optional and required adding NuGet package Microsoft.aspnetcore.mvc.formatters.xml
            services.AddMvc().AddXmlSerializerFormatters();

            // add a service that sets up a few of the numerous options when serializing to JSON with an API
            // the formatting can make output be more readable (while testing) but makes the responses bigger
            // the loophandling can be helpfull serializing EF entities directly when they reference eachother
            //services.AddMvc().AddJsonOptions(options =>
            //{
            //    options.SerializerSettings.Formatting = Formatting.Indented;
            //    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            //    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            //    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //});

            // add a service to allow API calls to work with simple caching options to increase performance
            services.AddResponseCaching();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Setup dependency injection of the correct DbContext so it can be injected into the controlers,
            // allowing for a generic setup and control of the scope of the DbContext by the container.
            // The connectionstring is setup in the appsettings.json file under that names property.
            var connection = Configuration.GetConnectionString("abSolutionsDatabase");
            services.AddDbContext<DatabaseContext>(options => options.UseLazyLoadingProxies()
                                                                     .UseSqlServer(connection));                       
        }

        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        { 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            // Needed to allow static HTML files to be used in the app, by default it will not do this !
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // 
            app.UseResponseCaching();

            app.UseMvc(routes =>
            {
                // Allow for an abrieviated URL call to get to the FieldsOfInterest controller. You need
                // to mention both controller routes and in the mentioned order, so the primairy is still
                // used for navigation and the URL does not show FoI instead of FieldsOfInterest.
                routes.MapRoute(
                    name: "FieldsOfInterest (primairy)",
                    template: "FieldsOfInterest/{action}/{id?}",
                    defaults: new { controller = "FieldsOfInterest", action = "Index", id = "" });
                routes.MapRoute(
                    name: "FieldsOfInterest (alternative)",                                        
                    template: "FoI/{action}/{id?}",                          
                    defaults: new { controller = "FieldsOfInterest", action = "Index", id = "" });

                // Default route for RESTfull controller. It is possible to add an {action} to this route also
                // to define the method, but by convention the methodname is derived from the HTTP verb (GET/POST/etc) 
                routes.MapRoute(
                    name: "Countries API",
                    template: "api/countries/{id?}",
                    defaults: new { controller = "CountriesApi", action = "", id = "" });

                // Default route for MVC controller
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                // URL that match the default route are for example
                //  - http://localhost:xxxx/Country/Edit/1?languageId=EN
                //  - http://localhost:xxxx/Country/Delete?id=1&languageId=EN

            });

            // Migrate (and possibly seed) the database during startup. Must be synchronous.
            using (var serviceScope = app.ApplicationServices
                                         .GetRequiredService<IServiceScopeFactory>()
                                         .CreateScope())
            {
                serviceScope.ServiceProvider.GetService<DatabaseContext>().Database.Migrate();
                //serviceScope.ServiceProvider.GetService<ISeedService>().SeedDatabase().Wait();
            }
        }
    }

    // Startup class for integration testing. Makes use of the SQLite in-memory database to access the data
    // which needs to be seeded when data is needed
    // Seems to only work if this class is defined in same project as Startup class. Don't know if
    // thats to do because of configuration file setting, assembly name resolution or finding controllers
    public class Startup2
    {
        public IConfiguration Configuration { get; }

        public Startup2(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Setup dependency injection for localization by use of IStringLocalizer<T> for the controller classes
            // who can then retrieve information per localization PER CONTROLLER. The accessor will try to look up
            // a dictionairy key in the file <ResourcePath>/Controller.<ControllerName>.<language>.resx or subdirectory
            // <ResourcePath>/Controller/<ControllerName>.<language>.resx. If not found it will return the key as string,
            // which allows to develop the app initially without worrying to much about adding resources.
            //services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

            // Setup dependency injection of a repository, so that controles can make use of these repositories
            // by accepting them in the constructor, while the generic setup and control of the scope is done
            // by the container. In turn the repository will take the below setup service for a DbContext into 
            // their constructor and no longer in the controler itself adding to seperation of concerns.
            services.AddScoped<ICrudRepositoryAsync<FieldOfInterestView, string>, FieldOfInterestViewRepository>();

            // add a service to allow API calls to return XML formatted data instead of the default JSON. This is
            // optional and required adding NuGet package Microsoft.aspnetcore.mvc.formatters.xml
            services.AddMvc().AddXmlSerializerFormatters();

            // add a service to allow API calls to work with simple caching options to increase performance
            services.AddResponseCaching();

            // add a service to implement memory caching. This adds a service which allows an IMemoryCache object
            // to be injected into wherever wanted (like the context), which can be used to store data in memory on 
            // the server and then retrieved at at a later time. Be sure to add implementations to limit size, 
            // invalidate caches when base data is updated (which can be tricking to pinpoint) or after a specific 
            // amount of time. Could be used in a repostiry where the GetAll result is cached for later use, while 
            // the calls with SaveChanges remove the cached data so it will be refreshed next time. Looks simple
            // enough, but take into consideration other people could add to the data or change related data which
            // would make the cached data obsolete. This is also true if a user leaves his browser open for a period
            // of time, so the software should be robust for obsolete data, but the user will expect data to be 
            // current if he changed any main or related data.
            services.AddMemoryCache();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Setup dependency injection of the correct DbContext so it can be injected into the controlers,
            // allowing for a generic setup and control of the scope of the DbContext by the container.
            // The connectionstring is setup in the appsettings.json file under that names property.
            //var connection = Configuration.GetConnectionString("abSolutionsDatabase");
            //services.AddDbContext<DatabaseContext>(options => options.UseLazyLoadingProxies()
            //                                                         .UseSqlServer(connection));

            //var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            //var connectionString = connectionStringBuilder.ToString();
            //var connection = new SqliteConnection(connectionString);

            // The SQLite memory database needs an open connection to work, so needs to be set up as a singleton, else the connection openend in configure closes to early
            services.AddDbContext<DatabaseContext>(options => options.UseLazyLoadingProxies()
                                                                     //.UseInMemoryDatabase("TEST"));
                                                                     .UseSqlite(new SqliteConnection("DataSource=:memory:")),ServiceLifetime.Singleton);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // 
            app.UseResponseCaching();

            app.UseMvc(routes =>
            {
                // Allow for an abrieviated URL call to get to the FieldsOfInterest controller. You need
                // to mention both controller routes and in the mentioned order, so the primairy is still
                // used for navigation and the URL does not show FoI instead of FieldsOfInterest.
                routes.MapRoute(
                    name: "FieldsOfInterest (primairy)",
                    template: "FieldsOfInterest/{action}/{id?}",
                    defaults: new { controller = "FieldsOfInterest", action = "Index", id = "" });
                routes.MapRoute(
                    name: "FieldsOfInterest (alternative)",
                    template: "FoI/{action}/{id?}",
                    defaults: new { controller = "FieldsOfInterest", action = "Index", id = "" });

                // Default route for RESTfull controller. It is possible to add an {action} to this route also
                // to define the method, but by convention the methodname is derived from the HTTP verb (GET/POST/etc) 
                routes.MapRoute(
                    name: "Countries API",
                    template: "api/countries/{id?}",
                    defaults: new { controller = "CountriesApi", action = "", id = "" });

                // Default route for MVC controller
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                // URL that match the default route are for example
                //  - http://localhost:xxxx/Country/Edit/1?languageId=EN
                //  - http://localhost:xxxx/Country/Delete?id=1&languageId=EN

            });

            // Migrate (and possibly seed) the database during startup. Must be synchronous.
            using (var serviceScope = app.ApplicationServices
                                         .GetRequiredService<IServiceScopeFactory>()
                                         .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DatabaseContext>();
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                //serviceScope.ServiceProvider.GetService<DatabaseContext>().Database.Migrate();
                //serviceScope.ServiceProvider.GetService<ISeedService>().SeedDatabase().Wait();
            }
        }
    }

}
