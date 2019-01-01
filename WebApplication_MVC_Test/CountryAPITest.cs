using DatabaseModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using WebApplication_MVC;

// Added a NuGet package Microsoft.AspNetCore.TestHost  
// For testing under asp.net core 2.1 also needed to add Microsoft.AspNetCore.App NuGet package and
// replace the header of my csproj file to <Project Sdk="Microsoft.NET.Sdk.Web"> (adding the ".Web")

namespace WebApplication_MVC_Test
{
    [TestClass]
    public class CountryAPITest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public CountryAPITest()
        {
            // only works with a teststartup class that is located in project of startup (and the controllers)
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup2>());
            _client = _server.CreateClient();

            SeedContextWithCountries();
        }

        [TestMethod]
        // Integration test by using a testserver and client instead of calling the controller directly
        public void CountryAPIGetAll()
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/countries");

            //Act
            var response = _client.SendAsync(request).Result;   // could also use the method GetAsync with just the URI
            //response.EnsureSuccessStatusCode();                 // throws an exception if http response statuscode out of 2xx range

            // deserializing answer from a list of countries
            var countries = response.Content.ReadAsAsync<List<Country>>().Result;
            //var responseData = response.Content.ReadAsStringAsync().Result; // for debugging            

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"HTTP Statuscode not {HttpStatusCode.OK}");
            Assert.AreEqual(countries.Count.ToString(), response.Headers.GetValues("X-Number-Of-Countries").SingleOrDefault(), "X-Number-Of-Countries header contains invalid value");  // GetValues returns an enumerable, so retrieve that !
            Assert.AreEqual("BE", countries[0].CountryId, "First country in testset not BE");
            Assert.AreEqual("Belgium", countries[0].Descriptions.First().Description, "First country in testset not Belgium");
            Assert.AreEqual("EN", countries[1].CountryId, "Second country in testset not EN");
            Assert.AreEqual("Niederlande", countries[2].Descriptions.First().Description, "Third country in testset not Niederlande");
        }

        private void SeedContextWithCountries()
        {
            var context = _server.Host.Services.GetService<DatabaseContext>();

            Country country = new Country() { CountryId = "NL" };
            country.Descriptions.Add(new CountryDescription() { CountryId = "NL", LanguageId = "NL", Description = "Nederland" });
            country.Descriptions.Add(new CountryDescription() { CountryId = "NL", LanguageId = "EN", Description = "Netherlands" });
            country.Descriptions.Add(new CountryDescription() { CountryId = "NL", LanguageId = "DE", Description = "Niederlande" });
            context.Add(country);

            country = new Country() { CountryId = "BE" };
            country.Descriptions.Add(new CountryDescription() { CountryId = "BE", LanguageId = "NL", Description = "Belgie" });
            country.Descriptions.Add(new CountryDescription() { CountryId = "BE", LanguageId = "EN", Description = "Belgium" });
            context.Add(country);

            country = new Country() { CountryId = "EN" };
            country.Descriptions.Add(new CountryDescription() { CountryId = "UK", LanguageId = "EN", Description = "United kingdom" });
            context.Add(country);

            context.SaveChanges();
        }
    }





    // MSDN testclass to API calls, which do not mock up a server, but call the API controller directly.
    // Their method creates a subclassed dbcontext, but a mocked up in memory one would do just fine.
    // This will not test routing !
    // https://docs.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/mocking-entity-framework-when-unit-testing-aspnet-web-api-2
    //[TestClass]
    //public class TestProductController
    //{
    //    [TestMethod]
    //    public void PostProduct_ShouldReturnSameProduct()
    //    {
    //        var controller = new ProductController(new TestStoreAppContext());

    //        var item = GetDemoProduct();

    //        var result =
    //            controller.PostProduct(item) as CreatedAtRouteNegotiatedContentResult<Product>;

    //        Assert.IsNotNull(result);
    //        Assert.AreEqual(result.RouteName, "DefaultApi");
    //        Assert.AreEqual(result.RouteValues["id"], result.Content.Id);
    //        Assert.AreEqual(result.Content.Name, item.Name);
    //    }

    //    [TestMethod]
    //    public void PutProduct_ShouldReturnStatusCode()
    //    {
    //        var controller = new ProductController(new TestStoreAppContext());

    //        var item = GetDemoProduct();

    //        var result = controller.PutProduct(item.Id, item) as StatusCodeResult;
    //        Assert.IsNotNull(result);
    //        Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
    //        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
    //    }

    //    [TestMethod]
    //    public void PutProduct_ShouldFail_WhenDifferentID()
    //    {
    //        var controller = new ProductController(new TestStoreAppContext());

    //        var badresult = controller.PutProduct(999, GetDemoProduct());
    //        Assert.IsInstanceOfType(badresult, typeof(BadRequestResult));
    //    }

    //    [TestMethod]
    //    public void GetProduct_ShouldReturnProductWithSameID()
    //    {
    //        var context = new TestStoreAppContext();
    //        context.Products.Add(GetDemoProduct());

    //        var controller = new ProductController(context);
    //        var result = controller.GetProduct(3) as OkNegotiatedContentResult<Product>;

    //        Assert.IsNotNull(result);
    //        Assert.AreEqual(3, result.Content.Id);
    //    }

    //    [TestMethod]
    //    public void GetProducts_ShouldReturnAllProducts()
    //    {
    //        var context = new TestStoreAppContext();
    //        context.Products.Add(new Product { Id = 1, Name = "Demo1", Price = 20 });
    //        context.Products.Add(new Product { Id = 2, Name = "Demo2", Price = 30 });
    //        context.Products.Add(new Product { Id = 3, Name = "Demo3", Price = 40 });

    //        var controller = new ProductController(context);
    //        var result = controller.GetProducts() as TestProductDbSet;

    //        Assert.IsNotNull(result);
    //        Assert.AreEqual(3, result.Local.Count);
    //    }

    //    [TestMethod]
    //    public void DeleteProduct_ShouldReturnOK()
    //    {
    //        var context = new TestStoreAppContext();
    //        var item = GetDemoProduct();
    //        context.Products.Add(item);

    //        var controller = new ProductController(context);
    //        var result = controller.DeleteProduct(3) as OkNegotiatedContentResult<Product>;

    //        Assert.IsNotNull(result);
    //        Assert.AreEqual(item.Id, result.Content.Id);
    //    }

    //    Product GetDemoProduct()
    //    {
    //        return new Product() { Id = 3, Name = "Demo name", Price = 5 };
    //    }
    //}
}
