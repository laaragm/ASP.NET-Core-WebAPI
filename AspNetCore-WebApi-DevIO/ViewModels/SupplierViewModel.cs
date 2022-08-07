using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore_WebApi_DevIO.ViewModels
{
	public class SupplierViewModel
	{
		[Key]
		public Guid Id { get; set; }

		[Required(ErrorMessage = "The field {0} is mandatory")]
		[StringLength(100, ErrorMessage = "The field {0} is mandatory and it should contain between {2} and {1} letters", MinimumLength = 2)]
		public string Name { get; set; }

		[Required(ErrorMessage = "The field {0} is mandatory")]
		[StringLength(14, ErrorMessage = "The field {0} is mandatory and it should contain between {2} and {1} letters", MinimumLength = 11)]
		public string Document { get; set; }

		public int SupplierType { get; set; }

		public AddressViewModel Address { get; set; }

		public IEnumerable<ProductViewModel> Products { get; set; }

		public bool IsActive { get; set; }
	}
}
