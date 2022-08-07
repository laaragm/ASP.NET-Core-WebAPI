using AspNetCore_WebApi_DevIO.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Http.ModelBinding;

namespace AspNetCore_WebApi_DevIO.ViewModels
{
    [ModelBinder(BinderType = typeof(ProductModelBinder))]
    public class ProductViewModel
	{
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory")]
        public Guid SupplierId { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory")]
        [StringLength(200, ErrorMessage = "The field {0} is mandatory and it should contain between {2} and {1} letters", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory")]
        [StringLength(1000, ErrorMessage = "The field {0} is mandatory and it should contain between {2} and {1} letters", MinimumLength = 2)]
        public string Description { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory")]
        public decimal Price { get; set; }

		[ScaffoldColumn(false)]
        public DateTime RegisterDate { get; set; }

        [ScaffoldColumn(false)]
        public string SupplierName { get; set; }

        public bool IsActive { get; set; }

        public IFormFile ImageUpload { get; set; }

        public string Image { get; set; }
    }
}
