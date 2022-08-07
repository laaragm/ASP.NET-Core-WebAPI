using AspNetCore_WebApi_DevIO.Extensions;
using AspNetCore_WebApi_DevIO.ViewModels;
using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using AspNetCore_WebAPI_DevIO.Business.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCore_WebApi_DevIO.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	public class ProductsController : MainController
	{
		private readonly IProductRepository ProductRepository;
		private readonly IProductService ProductService;
		private readonly IMapper Mapper;

		public ProductsController(INotifier notifier, 
								  IProductRepository productRepository,
								  IProductService productService,
								  IMapper mapper) : base(notifier)
		{
			ProductRepository = productRepository;
			ProductService = productService;
			Mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetAll()
		{
			var result = Mapper.Map<IEnumerable<ProductViewModel>>(await ProductRepository.GetProductsSuppliers());
			return Ok(result);
		}

		[HttpGet("{id:guid}")]
		public async Task<ActionResult<ProductViewModel>> GetById(Guid id)
		{
			var productViewModel = await GetProduct(id);
			if (productViewModel == null)
			{
				return NotFound();
			}

			return Ok(productViewModel);
		}

		[ClaimsAuthorize("Product", "Create")]
		[HttpPost]
		public async Task<ActionResult<ProductViewModel>> Create([ModelBinder(BinderType = typeof(ProductModelBinder))] ProductViewModel productViewModel)
		{
			if (!ModelState.IsValid)
			{
				return CustomResponse(ModelState);
			}

			var imagePrefix = Guid.NewGuid() + "_";
			if (!await UploadFile(productViewModel.ImageUpload, imagePrefix))
			{
				return CustomResponse(productViewModel);
			}
			productViewModel.Image = imagePrefix + productViewModel.ImageUpload.FileName;
			await ProductService.Add(Mapper.Map<Product>(productViewModel));

			return CustomResponse(productViewModel);
		}

		[ClaimsAuthorize("Product", "Delete")]
		[HttpDelete("{id:guid}")]
		public async Task<ActionResult<ProductViewModel>> Delete(Guid id)
		{
			var product = GetProduct(id);
			if (product == null)
			{
				return NotFound();
			}
			await ProductService.Delete(id);

			return CustomResponse(product);
		}

		[ClaimsAuthorize("Product", "Update")]
		[HttpPut("{id:guid}")]
		public async Task<IActionResult> Update(Guid id, [ModelBinder(BinderType = typeof(ProductModelBinder))] ProductViewModel productViewModel)
		{
			if (id != productViewModel.Id)
			{
				NotifyError("The informed id is not the same as the one in the query");
				return CustomResponse();
			}

			var productToBeUpdated = await GetProduct(id);
			if (string.IsNullOrEmpty(productViewModel.Image))
			{
				productViewModel.Image = productToBeUpdated.Image;
			}

			if (!ModelState.IsValid)
			{
				return CustomResponse(ModelState);
			}

			if (productViewModel.ImageUpload != null)
			{
				var imagePrefix = Guid.NewGuid() + "_";
				if (!await UploadFile(productViewModel.ImageUpload, imagePrefix))
				{
					return CustomResponse(productViewModel);
				}
				productToBeUpdated.Image = imagePrefix + productViewModel.ImageUpload.FileName;
			}

			productToBeUpdated.SupplierId = productViewModel.SupplierId;
			productToBeUpdated.Name = productViewModel.Name;
			productToBeUpdated.Description = productViewModel.Description;
			productToBeUpdated.Price = productViewModel.Price;
			productToBeUpdated.IsActive = productViewModel.IsActive;

			await ProductService.Update(Mapper.Map<Product>(productToBeUpdated));

			return CustomResponse(productViewModel);
		}

		private async Task<ProductViewModel> GetProduct(Guid id)
			=> Mapper.Map<ProductViewModel>(await ProductRepository.GetProductSupplier(id));

		private async Task<bool> UploadFile(IFormFile file, string imagePrefix)
		{
			if (file == null || file.Length == 0)
			{
				NotifyError("Please add an image for the product");
				return false;
			}

			var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", imagePrefix + file.FileName);

			if (System.IO.File.Exists(path))
			{
				NotifyError("A file with this name already exists");
				return false;
			}

			using (var stream = new FileStream(path, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			return true;
		}

		// The code below shows that you can upload just the file so you don't have to deal with the issue that made us create a Custom Model Binder for the Product
		//[RequestSizeLimit(40000000)] // file size limit: 40MB - you can also use [DisableRequestSizeLimit] but it's not appropriate
		//[HttpPost("image")]
		//public async Task<ActionResult<ProductViewModel>> AddImage(IFormFile file)
		//{
		//	return Ok(file);
		//}

		// The code below shows a solution for uploading a file using base64 - the property ImageUpload in this case would be a string
		//private bool UploadFile(string file, string imageName)
		//{
		//	if (string.IsNullOrEmpty(file))
		//	{
		//		NotifyError("Please add an image for the product");
		//		return false;
		//	}

		//	var imageDataByteArray = Convert.FromBase64String(file);
		//	var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imageName);
		//	if (System.IO.File.Exists(filePath)) {
		//		NotifyError("A file with this name already exists");
		//		return false;
		//	}

		//	System.IO.File.WriteAllBytes(filePath, imageDataByteArray);

		//	return true;
		//}
	}
}
