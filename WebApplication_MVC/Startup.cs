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

namespace WebApplication_MVC
{
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
            app.UseStaticFiles();
            app.UseCookiePolicy();
            
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
}
