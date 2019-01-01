using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DatabaseModel
{
    // Attribute used by serializers when returing country object data (Json OR XML). If only using Json
    // you would think just using the JsonIgnore attribute might be sufficient, but it is always better 
    // for control to whitelist instead of blacklist. Here the problem arrises that EF adds properties 
    // not wanted in the export, but also not able to ignore, making for a crappy export. So best to use
    // data contracts to have maximum control and the best tuned result.
    [DataContract]
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
        // Attribute to indicate this property should be used with object serialization
        [DataMember(Order = 0, IsRequired = true)]
        // setter added to make sure the new countries only use uppercase values
        public string CountryId { get => _countryId; set { _countryId = value.ToUpper(); } }

        // notmapped means the property is not stored in database using EF
        [NotMapped]
        // stringlength added for (client) model validation (but should refer to lenght of countrydescription value)
        [StringLength(80)]        
        // do not serialize this property in Json for the WebAPI calls, it is derived information
        [JsonIgnore]  // not needed when using with datacontracts who uses whitelisting the datamembers
        // do not serialize this property in Json or XML for the WebAPI calls, it is derived information
        [IgnoreDataMember]  // not needed when using with datacontracts who uses whitelisting the datamembers
        public string Description { get => DescriptionHelper.GetDescription(Descriptions, out _languageId); set { } }

        // notmapped means the property is not stored in database using EF
        [NotMapped]
        // do not serialize this property in Json for the WebAPI calls, it is derived information
        [JsonIgnore]  // not needed when using with datacontracts who uses whitelisting the datamembers
        public string LanguageId { get => _languageId; }

        // Attribute to indicate this property should be used with object serialization
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public virtual ICollection<CountryDescription> Descriptions { get; set; }


        // Method called before serializing data contracts. Could be used to transform unsupported types to
        // information that can be serialized, just before the serialization starts
        [OnSerializing]
        private void SetValuesOnSerializing(StreamingContext context)
        {
        }

        // Method called after serializing data contracts. Could be used to free up resources used for 
        // serializing only.
        [OnSerialized()]
        private void SetValuesOnSerialized(StreamingContext context)
        {
        }

        // Method called before deserializing data contracts. Could/should be used to initialize values in
        // the object that are set by value or the constructor, because deserializing does NOT call any of
        // the constructor or set any default values of properties. Other means of deserializing have 
        // different ways to do this, like using a specific constructor with a specific parameter set.
        [OnDeserializing]
        private void SetValuesOnDeserializing(StreamingContext context)
        {
        }

        // Method called after deserializing data contracts. Could/should be used to set derived properties 
        // or check if all properties needed for proper processing were set to valid values.
        [OnDeserialized]
        private void SetValuesOnDeserialized(StreamingContext context)
        {
        }
    }

    [DataContract]
    public class CountryDescription : GeneralDescription
    {
        public string CountryId { get; set; }

        // do not serialize this property in Json for the WebAPI calls, it is derived information
        [JsonIgnore]  // not adding this one causes a problem when serializing countries whe not using datacontracts
        public virtual Country Country { get; set; }
    }

    [DataContract]
    public class GeneralDescription
    {
        [DataMember(IsRequired = true)]
        public string LanguageId { get; set; }
        [DataMember(EmitDefaultValue = false)]
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
