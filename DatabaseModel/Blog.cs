﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseModel
{
    // Data annotations adds attributes to classes, properties and methods and can be used to add information
    // that other processes can get ahold off to overrule the default behavior, specifically ASP.NET MVC and EF.
    // Not all data annotations present for EF 6 are implemented in EF core 2 so consider using fluent API.

    // [Table("xxx"{,Schema="yyy"})] 
    //   overrules the class name with the specified name, Schema optionally overrules the default 'dbo' schema
    // [Column("xxx"{,Order=nnn}{,TypeName="yyy"})]
    //   overrules the property name with the specified name, Order optionally overrules the order of the fields in a db record (must be used on all properties), TypeName optionally overrules the implicite type of the field in a db record
    // [ForeignKey("xxx")]
    //   used if help creating a foreign key when the propertyname does not match the key property of the intended linked class (probably due to bad naming convention)
    // [InverseProperty("xxx")]
    //  helps where a class has multiple foreign key relationships with another class to specify which property should link to which foreign property (see documentation for details)

    // [Key]
    //   indicates which property is the primairy key. If the primairy key consists of multiple properties, use fluent API since not supported in EF core (only in EF 6)
    // [Timestamp]
    //   a byte[] property (one per class) which will get the timestamp data type in database and used in EF for concurrency checks
    // [ConcurrencyCheck]
    //   indicates if a property should be used in the concurrency check when updating a record in the database, more properties can have this attribute where you can only have one [Timestamp] attribute 
    // [NotMapped]
    //   property (or class) should not be mapped to a database field (a property without getter or setter will also never be mapped)
    // [DatabaseGenerated(DatabaseGenerationOption.Computed/Identity/None]
    //   property value is generated by the database when it is added to the database, or when it is added or updated (saved), e.g. keyid or creation date (Identity option), rowversion or last accessed date (Computed option). None option used to overrule the default EF generates for Id key properties
    // [Required]
    //   property cannot have a NULL value in the database (and aids in automated model validation on web application)
    // [MinLength(nnn)]
    //   add to a string or byte[] properties it to have it minimum length validated when updating (and aids in automated model validation on web application)
    // [MaxLength(nnn)] (or [StringLength(nnn)] for strings only)
    //   add to a string or byte[] properties it created a limited database field definition and validate it when updating (and aids in automated model validation on web application)

    // NOT supported in EF core 2.0 (as far as i know), but supported in EF 6

    // [Index({"xxx"{,n}}{,IsUnique=true}{,IsClustered=true})]
    //   makes sure an index is added for the property, optionally with a specified name, that optionally can be either an unique or clustered key
    //   making one index that includes multiple properies required a name to be given and the order in which the property occurs in the key, eq [Index("IndexName", 1)]
    //   a unique index just states there can be only one key value present at any time, speeding up queries because reading can stop if found
    //   a clustered index (one per table) is the physical storage order of the data and the fastests to query, all other non-clustered indexes are stored seperately with a 
    //    row identifier to the original data (in the clustered index). Including additional (redundant data) columns to a non-clustered key might reduce the need to access
    //    the table data and therefor speed up query performance, but comes at the cost of more storage and perhaps slower key access. Design smartly!
    // [ComplexType]
    //   a property not just a simple scalar, like a struct of some sort


    // ASP.NET MVC model validation data annotations

    // [Required]
    //   Indicated that a property is a required field
    // [MinLength(nnn)]
    //   Specifies the minimum length of a string property
    // [MaxLength(nnn)] or [StringLength(nnn)]
    //   Specifies the maximum length of a string property
    // [Range(nnn,mmm)]
    //   Specifies a minimum and maximum value for a numeric property
    // [RegularExpression("xxx")]
    //   Specifies that the property value must match a specific regular expression
    // [Phone] [EmailAdress] [CreditCard] [FileExtension("xxx")]
    //   Specifies that the property value must match a specific format for either 
    //   phone numbers, e-mail adresses, credit card numbers or filenames
    // [CustomValidation]
    //   Specified custom validation method to validate the property value

    // Also see http://www.tutorialsteacher.com/mvc/implement-validation-in-asp.net-mvc

    // the [Display(Name="xxx")] data annotation attribute allows for an alias of a property name for presentation on a web application

    [Table("Blogs")]                                            // = default by EF naming convention (plural not singular!)
    public class Blog
    {
        public Blog()
        {
            this.Posts = new HashSet<Post>();
        }
         
        [Key]                                                   // = default by EF naming convention
        [Column("BlogId")]                                      // = default by EF naming convention
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   // = default by EF naming convention
        public int BlogId { get; set; }
        [Required]                                              
        [MinLength(3)]
        [MaxLength(80)]
        [Display(Name = "Description")]                         // = default by EF naming convention
        public string Description { get; set; }
        [Column(TypeName = "DateTime")]
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }

        [Timestamp]
        [ConcurrencyCheck]                                      // = default for a Timestamp property
        public byte[] RowVersion { get; set; }

        [NotMapped]
        public int PostCounter { get; set; }

        // the virtual keyword allows for lazy loading of the Posts related to this blog
        public virtual ICollection<Post> Posts { get; set; }
    }
}
