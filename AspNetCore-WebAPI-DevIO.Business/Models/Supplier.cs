using AspNetCore_WebAPI_DevIO.Business.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore_WebAPI_DevIO.Business.Models
{
	public class Supplier : Entity
	{
        public string Name { get; set; }
        public string Document { get; set; }
        public SupplierType SupplierType { get; set; }
        public Address Address { get; set; }
        public bool IsActive { get; set; }

        // EF Relations
        public IEnumerable<Product> Products { get; set; }
    }
}
