using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseModel
{
    public class Contact
    {
        public Contact()
        {
            this.DeliveryAddresses = new HashSet<Address>();
        }

        public int ContactId { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string LanguageId { get; set; }
        public string AlternativeLanguageId { get; set; }

        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdated { get; set; }
        public string LastUpdatedBy { get; set; }

        public byte[] RowVersion { get; set; }

        public int VisitingAddressId { get; set; }
        public virtual Address VisitingAddress { get; set; }

        public int PostalAddressId { get; set; }
        public virtual Address PostalAddress { get; set; }

        public virtual ICollection<Address> DeliveryAddresses { get; set; }
    }
}
