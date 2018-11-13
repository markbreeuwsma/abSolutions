using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using DatabaseModel;

namespace WebApplication_MVC.Models
{
    public class FieldOfInterestView
    {
        public FieldOfInterestView()
        {
            LanguageId = DescriptionHelper.UserLanguageId;
            Created = DateTime.Now;
            CreatedBy = "Anonymous";   //TODO retrieve user name ?
        }

        [Required]
        [StringLength(5)]
        [Display(Name ="Field of interest")]
        [Remote(action: "RemoteValidateFieldOfInterestId", controller: "FieldsOfInterest")]
        public string FieldOfInterestId { get; set; }   
        [Required]
        [StringLength(2)]        
        [HiddenInput(DisplayValue = false)]
        public string LanguageId { get; set; }
        [StringLength(80)]
        public string Description { get; set; }
        [DataType(DataType.Date)]                // [DisplayFormat(DataFormatString = "{0:d}")] does the same
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
    }
}
