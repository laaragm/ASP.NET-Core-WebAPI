using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore_WebAPI_DevIO.Business.Models
{
	public class Address : Entity
	{
        public Guid SupplierId { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Adjunct { get; set; }
        public string Cep { get; set; }
        public string Neighbourhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        // EF Relation
        public Supplier Supplier { get; set; }
    }
}
