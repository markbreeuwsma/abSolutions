using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using DatabaseModel;
using WebApplication_MVC.Resources;

namespace WebApplication_MVC.Models
{
    public class FieldOfInterestView
    {
        public FieldOfInterestView()
        {
            LanguageId = DescriptionHelper.UserLanguageId;
            Created = DateTime.Now;
            CreatedBy = "Anonymous";   //TODO retrieve user name 
        }

        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(ErrorMessage))]  
        [StringLength(5, ErrorMessageResourceName = "FieldMaxLength", ErrorMessageResourceType = typeof(ErrorMessage))]  // Alternative error message
        [Display(Name = "FieldOfInterestId", ResourceType = typeof(ErrorMessage))]
        [Remote(action: "RemoteValidateFieldOfInterestId", controller: "FieldsOfInterest")]
        public string FieldOfInterestId { get; set; }

        [Required]  
        [StringLength(2)]        
        [HiddenInput(DisplayValue = false)]
        public string LanguageId { get; set; }

        [StringLength(80, ErrorMessageResourceName = "FieldOfInterestDescriptionMaxLength", ErrorMessageResourceType = typeof(ErrorMessage))]
        // [RegularExpression(@"^[A-Z\s].*", ErrorMessage = "Description needs to start with a capital")]  // Just an example, not very nice to force format on descriptions tho, better for alfanumeric Id's
        // [DisplayFormat(NullDisplayText = "Unknown....")]  // A description can be empty, but might be usefull for other nullable instances
        public string Description { get; set; }

        [DataType(DataType.Date)]                // simular to [DisplayFormat(DataFormatString = "{0:d}")], but DataType can adds additional options to HTML that DispayFormat won't
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",ApplyFormatInEditMode = true)]     // specifying a specific format will overrule culture presentation
        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]                // simular to [DisplayFormat(DataFormatString = "{0:d}")], but DataType can adds additional options to HTML that DispayFormat won't
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]     // specifying a specific format will overrule culture presentation
        public DateTime Updated { get; set; }

        public string UpdatedBy { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
