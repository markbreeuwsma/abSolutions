using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseModel
{
    public class Country
    {
        public Country()
        {
            this.Descriptions = new HashSet<CountryDescription>();
        }

        private string _countryId;
        private string _languageId;

        // required added for (client) model validation
        [Required]
        // stringlength added for (client) model validation
        [StringLength(2)]
        // displayname added for more user friendly presentation without the Id suffix
        [Display(Name = "Country")]
        // remote validation added to check if already existst when entering code at client (needs microsoft.aspnetcore.mvc package)
        [Remote(action: "ValidateCountryId", controller: "Countries")]
        // setter added to make sure the new countries only use uppercase values
        public string CountryId { get => _countryId; set { _countryId = value.ToUpper(); } }

        // notmapped means the property is not stored in database using EF
        [NotMapped]
        // stringlength added for (client) model validation (but should refer to lenght of countrydescription value)
        [StringLength(80)]
        public string Description { get => DescriptionHelper.GetDescription(Descriptions, out _languageId); set { } }

        // notmapped means the property is not stored in database using EF
        [NotMapped]
        public string LanguageId { get => _languageId; }

        public virtual ICollection<CountryDescription> Descriptions { get; set; }
    }

    public class CountryDescription : GeneralDescription
    {
        public string CountryId { get; set; }

        public virtual Country Country { get; set; }
    }

    public class GeneralDescription
    {
        public string LanguageId { get; set; }
        public string Description { get; set; }
    }

    public static class DescriptionHelper
    {
        public static string UserLanguageId { get { return "NL";  } }     // TODO retrieve language from user 
        public static string SystemLanguageId { get { return "EN";  } }   // TODO retrieve language from system (localization)

        public static string GetDescription(IEnumerable<GeneralDescription> descriptions, out string languageId)
        {
            // TODO define fallback mechanisme
            // when showing fallback is ok, when editing, no fallback is wanted, when reporting report or contact language is wanted 

            string firstLang = UserLanguageId;      
            string secondLang = SystemLanguageId;  
            string secondLangDescr = "";

            if (descriptions != null)
            {
                using (var enumerator = descriptions.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.LanguageId == firstLang)
                        {
                            languageId = firstLang;
                            return enumerator.Current.Description;
                        }
                        if (enumerator.Current.LanguageId == secondLang)
                        {
                            secondLangDescr = enumerator.Current.Description;
                        }
                    }
                }
            }

            languageId = secondLang;
            return secondLangDescr;
        }
    }
}
