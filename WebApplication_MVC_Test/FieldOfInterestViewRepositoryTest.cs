using DatabaseModel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using WebApplication_MVC.Models;

// Added a NuGet package Microsoft.AspNetCore.TestHost
// Added a NuGet package Microsoft.Data.Sqlite and Microsoft.EntityFrameworkCore.Sqlite
// For testing under asp.net core 2.1 also needed to add Microsoft.AspNetCore.App NuGet package and
// replace the header of my csproj file to <Project Sdk="Microsoft.NET.Sdk.Web"> (adding the ".Web")

namespace WebApplication_MVC_Test
{
    // Contains testing methods which use InMemory and SqliteInMemory context's. The plain InMemory tests will not
    // check various relational database requirements that the SQLite memory context will (since it will actually
    // create a relational database, just very temporary). Using the SQLite memory context is therefor prefered,
    // since it potentially will uncover more problems, altho it does come at a cost of some performance.
    // Dummy context's does require the test to fill the context with the appropriate data, but that is true for all
    // testing methods, altho they might use different methods.

    // Instead of using dummy context's and feeding those into the repositories, another strategy is to create mocked
    // up repositories and not even passing them a context. They would set up a private list or dictionary property and
    // mimic the interfaced methods by working on the list. 
    // For simple repositories this feels like superflues to previous method of a dummy context, since it requires 
    // writing out a new mocked up repository while missing out on relational database checks that you are not going
    // to implement. But where it can be usefull is if a repository does a lot more then just database actions, which
    // are not wanted in a testing environment. Also dummy repositories can be a lot faster then using a dummy context,
    // but like described, they also do less. Could also be considered when testing for something other then the actual
    // database actions, but I think I still prefer a dummy context.

    // Instead of writing out an actual new class for a mocked up repository, there are frameworks like MOQ, which 
    // allows virtual mocked up repositories by encapsulating an interface in the Mock<ISomeinferface> class. Altho
    // there is no need for an extra class, the responses to method calls still need to be coded, and this is then 
    // done in the testmethod itself. It makes a test more contained, but the setup takes some getting comfertable
    // with (e.g. MockedRepo.Setup(x => x.method(param values)).Result(output)). Using this method of mocking requires 
    // adding NuGet package Moq.  

    [TestClass]
    public class FieldOfInterestViewRepositoryTest
    {
        [TestMethod]
        public async Task DeleteFieldOfInterestInMemory()
        {
            // Arrange the test against one instance of the context (name the database to match the method)
            using (var context = ContextHelper.GetInMemoryContext("DeleteFieldOfInterest"))  
            {
                await SeedContextWithFieldsOfInterest(context);

                var view = new FieldOfInterestView() { FieldOfInterestId = "01" };
                var repo = new FieldOfInterestViewRepository(context);

                await repo.DeleteAsync(view);
            }

            // Assert a separate instance of the context to verify correct data was updated in the named database
            using (var context = ContextHelper.GetInMemoryContext("DeleteFieldOfInterest"))
            {
                var field = context.Find<FieldOfInterest>("01");
                Assert.IsNull(field, "The field of interest 01 should be deleted");

                var fieldDescription = context.Find<FieldOfInterestDescription>("01","NL");
                Assert.IsNull(fieldDescription, "The field of interest 01 description NL should be deleted");

                fieldDescription = context.Find<FieldOfInterestDescription>("01", "EN");
                Assert.IsNull(fieldDescription, "The field of interest 01 description EN should be deleted");

                field = context.Find<FieldOfInterest>("02");
                Assert.AreEqual(context.Entry(field).State, EntityState.Unchanged, "The field of interest 02 should be marked unchanged");

                fieldDescription = context.Find<FieldOfInterestDescription>("02", "NL");
                Assert.AreEqual(context.Entry(fieldDescription).State, EntityState.Unchanged, "The field of interest 02 description NL should be marked unchanged");

                fieldDescription = context.Find<FieldOfInterestDescription>("02", "EN");
                Assert.AreEqual(context.Entry(fieldDescription).State, EntityState.Unchanged, "The field of interest 02 description EN should be marked unchanged");
            }
        }

        [TestMethod]
        public async Task DeleteFieldOfInterestInSQLiteMemory()
        {
            // the SQLite in-memory database only exists while the connection is open
            ContextHelper.OpenSQLiteInMemory();

            try
            {
                // Arrange the test against one instance of the context (name the database to match the method)
                using (var context = ContextHelper.GetSQLiteInMemoryContext())
                {
                    await SeedContextWithFieldsOfInterest(context);

                    var view = new FieldOfInterestView() { FieldOfInterestId = "01" };
                    var repo = new FieldOfInterestViewRepository(context);

                    // Act
                    await repo.DeleteAsync(view);
                }

                // Assert a separate instance of the context to verify correct data was updated in the named database
                using (var context = ContextHelper.GetSQLiteInMemoryContext())
                {
                    var field = context.Find<FieldOfInterest>("01");
                    Assert.IsNull(field, "The field of interest 01 should be deleted");

                    var fieldDescription = context.Find<FieldOfInterestDescription>("01", "NL");
                    Assert.IsNull(fieldDescription, "The field of interest 01 description NL should be deleted");

                    fieldDescription = context.Find<FieldOfInterestDescription>("01", "EN");
                    Assert.IsNull(fieldDescription, "The field of interest 01 description EN should be deleted");

                    field = context.Find<FieldOfInterest>("02");
                    Assert.AreEqual(context.Entry(field).State, EntityState.Unchanged, "The field of interest 02 should be marked unchanged");

                    fieldDescription = context.Find<FieldOfInterestDescription>("02", "NL");
                    Assert.AreEqual(context.Entry(fieldDescription).State, EntityState.Unchanged, "The field of interest 02 description NL should be marked unchanged");

                    fieldDescription = context.Find<FieldOfInterestDescription>("02", "EN");
                    Assert.AreEqual(context.Entry(fieldDescription).State, EntityState.Unchanged, "The field of interest 02 description EN should be marked unchanged");
                }
            }
            finally
            {
                ContextHelper.CloseSQLiteInMemory();
            }
        }

        // Can be a good idee to have a generic seeder method to if there are lots of database operations to check.
        // Makes the code a bit more readable, but on the downside is changing the seeded function should mean a
        // rerun all the tests using the seeder function and perhaps updating some.
        private static async Task SeedContextWithFieldsOfInterest(DatabaseContext context)
        {
            var field = new FieldOfInterest() { FieldOfInterestId = "01", Created = DateTime.Now, CreatedBy = "Anonymous", Updated = DateTime.Now, UpdatedBy = "" };
            field.Descriptions.Add(new FieldOfInterestDescription() { FieldOfInterestId = "01", LanguageId = "NL", Description = "Field 01/NL" });
            field.Descriptions.Add(new FieldOfInterestDescription() { FieldOfInterestId = "01", LanguageId = "EN", Description = "Field 01/EN" });
            context.Add(field);

            field = new FieldOfInterest() { FieldOfInterestId = "02", Created = DateTime.Now, CreatedBy = "Anonymous", Updated = DateTime.Now, UpdatedBy = "" };
            field.Descriptions.Add(new FieldOfInterestDescription() { FieldOfInterestId = "02", LanguageId = "NL", Description = "Field 02/NL" });
            field.Descriptions.Add(new FieldOfInterestDescription() { FieldOfInterestId = "02", LanguageId = "EN", Description = "Field 02/EN" });
            field.Descriptions.Add(new FieldOfInterestDescription() { FieldOfInterestId = "02", LanguageId = "DE", Description = "Field 02/DE" });
            context.Add(field);

            await context.SaveChangesAsync();

            // Checks a redundant, since hope to assume EF is well tested
            field = context.Find<FieldOfInterest>("01");
            Assert.AreEqual(context.Entry(field).State, EntityState.Unchanged, "The field of interest 01 should be marked unchanged");

            var fieldDescription = context.Find<FieldOfInterestDescription>("01", "NL");
            Assert.AreEqual(context.Entry(fieldDescription).State, EntityState.Unchanged, "The field of interest 01 description NL should be marked unchanged");

            fieldDescription = context.Find<FieldOfInterestDescription>("01", "EN");
            Assert.AreEqual(context.Entry(fieldDescription).State, EntityState.Unchanged, "The field of interest 01 description EN should be marked unchanged");

            fieldDescription = context.Find<FieldOfInterestDescription>("01", "DE");
            Assert.IsNull(fieldDescription, "The field of interest 01 description DE should not exist");

            field = context.Find<FieldOfInterest>("02");
            Assert.AreEqual(context.Entry(field).State, EntityState.Unchanged, "The field of interest 02 should be marked unchanged");

            fieldDescription = context.Find<FieldOfInterestDescription>("02", "NL");
            Assert.AreEqual(context.Entry(fieldDescription).State, EntityState.Unchanged, "The field of interest 02 description NL should be marked unchanged");

            fieldDescription = context.Find<FieldOfInterestDescription>("02", "EN");
            Assert.AreEqual(context.Entry(fieldDescription).State, EntityState.Unchanged, "The field of interest 02 description EN should be marked unchanged");

            fieldDescription = context.Find<FieldOfInterestDescription>("02", "DE");
            Assert.AreEqual(context.Entry(fieldDescription).State, EntityState.Unchanged, "The field of interest 02 description DE should be marked unchanged");
        }
    }
}
