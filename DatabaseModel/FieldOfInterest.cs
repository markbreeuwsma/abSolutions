using System;
using System.Collections.Generic;

namespace DatabaseModel
{
    public class FieldOfInterest
    {
        public FieldOfInterest()
        {
            this.Descriptions = new HashSet<FieldOfInterestDescription>();
        }

        public string FieldOfInterestId { get; set; }

        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<FieldOfInterestDescription> Descriptions { get; set; }
    }

    public class FieldOfInterestDescription : GeneralDescription
    {
        public string FieldOfInterestId { get; set; }

        public virtual FieldOfInterest FieldOfInterest { get; set; }
    }
}
