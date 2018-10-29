using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DatabaseModel
{
    public class Post
    {
        public int PostId { get; set; }
        public int BlogId { get; set; }
        [Required]
        public string Description { get; set; }
        public string Content { get; set; }
        public DateTime Posted { get; set; }
        public string PostedBy { get; set; }

        public virtual Blog Blog { get; set; }
    }
}
