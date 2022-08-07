using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore_WebApi_DevIO.ViewModels
{
	public class AddressViewModel
	{
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory")]
        [StringLength(200, ErrorMessage = "The field {0} is mandatory and it should contain between {2} and {1} letters", MinimumLength = 2)]
        public string Street { get; set; }


        [Required(ErrorMessage = "The field {0} is mandatory")]
        [StringLength(50, ErrorMessage = "The field {0} is mandatory and it should contain between {2} and {1} letters", MinimumLength = 1)]
        public string Number { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory")]
        [StringLength(100, ErrorMessage = "The field {0} is mandatory and it should contain between {2} and {1} letters", MinimumLength = 2)]
        public string Neighbourhood { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory")]
        [StringLength(8, ErrorMessage = "The field {0} is mandatory and it should contain between {2} and {1} letters", MinimumLength = 8)]
        public string Cep { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory")]
        [StringLength(100, ErrorMessage = "The field {0} is mandatory and it should contain between {2} and {1} letters", MinimumLength = 2)]
        public string City { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory")]
        [StringLength(50, ErrorMessage = "The field {0} is mandatory and it should contain between {2} and {1} letters", MinimumLength = 2)]
        public string State { get; set; }

        public string Adjunct { get; set; }

        public Guid SupplierId { get; set; }
    }
}
