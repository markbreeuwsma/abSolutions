using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication_MVC.Models;

using DatabaseModel;

namespace WebApplication_MVC.Controllers
{
    public class HomeController : Controller
    {
        // The DbContext is passed on from the container processing the web application and this way regulates
        // the scope of the context, improving maintainability and testing options. It is saved in a private 
        // field from the constructor so all other methods can make use of it. See startup.cs for the setup.
        // An injected DbContext might also be accesses in the following manner (although uncommon)
        //  - using (var context = serviceProvider.GetService<DatabaseContext>()) { ... }
        private readonly DatabaseContext _context;

        public HomeController(DatabaseContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
