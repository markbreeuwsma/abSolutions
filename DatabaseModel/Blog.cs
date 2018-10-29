using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DatabaseModel
{
    public class Blog
    {
        public Blog()
        {
            this.Posts = new HashSet<Post>();
        }

        // the [Required] data annotation attribute aids in automated model validation on web application
        // the [Display(Name="")] data annotation attribute allows for an alias of a property name for presentation on a web application

        public int BlogId { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }

        // the virtual keyword allows for lazy loading of the Posts related to this blog
        public virtual ICollection<Post> Posts { get; set; }
    }
}
