using System;
using System.Linq;

namespace DatabaseModel
{
    public static class DatabaseInitializer
    {
        public static void Initialize(DatabaseContext context)
        {
            context.Database.EnsureCreated();
           
            if (!context.Countries.Any())
            {
                context.Countries.AddRange(
                    new Country { CountryId = "NL" },
                    new Country { CountryId = "DE" },
                    new Country { CountryId = "BE" });

                context.CountryDescriptions.AddRange(
                    new CountryDescription { CountryId = "NL", LanguageId = "NL", Description = "Nederland" },
                    new CountryDescription { CountryId = "NL", LanguageId = "EN", Description = "The Netherlands" },
                    new CountryDescription { CountryId = "DE", LanguageId = "NL", Description = "Duitsland" },
                    new CountryDescription { CountryId = "DE", LanguageId = "EN", Description = "Germany" },
                    new CountryDescription { CountryId = "DE", LanguageId = "DE", Description = "Deutchland" },
                    new CountryDescription { CountryId = "BE", LanguageId = "EN", Description = "Belgium" });

                context.SaveChanges();
            }

            if (!context.FieldsOfInterest.Any())
            {
                context.FieldsOfInterest.AddRange(
                    new FieldOfInterest { FieldOfInterestId = "01", Created = DateTime.Parse("2018-10-01"), CreatedBy = "Anonymous", UpdatedBy = "" },
                    new FieldOfInterest { FieldOfInterestId = "02", Created = DateTime.Parse("2018-10-02"), CreatedBy = "Anonymous", UpdatedBy = "" },
                    new FieldOfInterest { FieldOfInterestId = "03", Created = DateTime.Parse("2018-11-01"), CreatedBy = "Anonymous", UpdatedBy = "" });

                context.FieldOfInterestDescriptions.AddRange(
                    new FieldOfInterestDescription { FieldOfInterestId = "01", LanguageId = "NL", Description = "Interesse 01" },
                    new FieldOfInterestDescription { FieldOfInterestId = "01", LanguageId = "EN", Description = "Field of interest 01" },
                    new FieldOfInterestDescription { FieldOfInterestId = "02", LanguageId = "NL", Description = "Interesse 02" },
                    new FieldOfInterestDescription { FieldOfInterestId = "03", LanguageId = "EN", Description = "Field of interest 03" });

                context.SaveChanges();
            }
        }
    }
}