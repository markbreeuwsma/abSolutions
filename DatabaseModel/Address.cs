using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseModel
{
    public enum AddressType { VisitingAddress = 1, PostalAddress = 2, DeliveryAddress = 4 };

    public class Address
    {
        public Address(AddressType type)
        {
            this.Type = type;
        }

        public int AddressId { get; set; }
        public AddressType Type { get; set; }
        public string Street { get; set; }
        public int HouseNo { get; set; }
        public string Addition { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CountryId { get; set; }

        public virtual Country Country { get; set; }

        // field needed to setup correct EF mapping
        public int? VisitingAddressContactId { get; set; }
        public virtual Contact VisitingAddressContact { get; set; }
        public int? PostalAddressContactId { get; set; }
        public virtual Contact PostalAddressContact { get; set; }
        public int? DeliveryAddressContactId { get; set; }
        public virtual Contact DeliveryAddressContact { get; set; }
    }

    public class VisitingAddress : Address
    {
        public VisitingAddress() : base(AddressType.VisitingAddress)
        {
        }
    }

    public class PostalAddress : Address
    {
        public PostalAddress() : base(AddressType.PostalAddress)
        {
        }
    }

}
