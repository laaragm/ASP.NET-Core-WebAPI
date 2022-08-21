using AspNetCore_WebApi_DevIO.Extensions;
using AspNetCore_WebApi_DevIO.ViewModels;
using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using AspNetCore_WebAPI_DevIO.Business.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore_WebApi_DevIO.Controllers
{
	[Authorize]
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}/[controller]")]
	public class SuppliersController : MainController
	{
		private readonly ISupplierRepository SupplierRepository;
		private readonly IMapper Mapper;
		private readonly ISupplierService SupplierService;
		private readonly IAddressRepository AddressRepository;

		public SuppliersController(ISupplierRepository supplierRepository, 
								   IMapper mapper, 
								   ISupplierService supplierService, 
								   INotifier notifier, 
								   IAddressRepository addressRepository,
								   IUser user) : base(notifier, user)
		{
			SupplierRepository = supplierRepository;
			Mapper = mapper;
			SupplierService = supplierService;
			AddressRepository = addressRepository;
		}

		//[AllowAnonymous]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<SupplierViewModel>>> GetAll()
		{
			var suppliers = Mapper.Map<IEnumerable<SupplierViewModel>>(await SupplierRepository.GetAll());

			return Ok(suppliers);
		}

		[HttpGet("{id:guid}")]
		public async Task<ActionResult<SupplierViewModel>> GetById(Guid id)
		{
			var supplier = Mapper.Map<SupplierViewModel>(await SupplierRepository.GetSuppliersProductsAddress(id));
			if (supplier == null)
			{
				return NotFound();
			}

			return Ok(supplier);
		}

		[ClaimsAuthorize("Supplier", "Create")]
		[HttpPost]
		public async Task<ActionResult<SupplierViewModel>> Create(SupplierViewModel supplierViewModel)
		{
			if (!ModelState.IsValid)
			{
				return CustomResponse(ModelState);
			}
			await SupplierService.Add(Mapper.Map<Supplier>(supplierViewModel));

			return CustomResponse(supplierViewModel);
		}

		[ClaimsAuthorize("Supplier", "Update")]
		[HttpPut("{id:guid}")]
		public async Task<ActionResult<SupplierViewModel>> Update(Guid id, SupplierViewModel supplierViewModel)
		{
			if (id != supplierViewModel.Id)
			{
				NotifyError("The informed id is not the same as the one in the query");
				return CustomResponse(supplierViewModel);
			}
			if (!ModelState.IsValid)
			{
				return CustomResponse(ModelState);
			}
			await SupplierService.Update(Mapper.Map<Supplier>(supplierViewModel));

			return CustomResponse(supplierViewModel);
		}

		[ClaimsAuthorize("Supplier", "Delete")]
		[HttpDelete("{id:guid}")]
		public async Task<ActionResult<SupplierViewModel>> Delete(Guid id)
		{
			var supplierViewModel = Mapper.Map<SupplierViewModel>(await SupplierRepository.GetSuppliersAddress(id));
			if (supplierViewModel == null)
			{
				return NotFound();
			}
			await SupplierService.Delete(id);

			return CustomResponse(supplierViewModel);
		}

		[HttpGet("get-address/{id:guid}")]
		public async Task<AddressViewModel> GetAddressById(Guid id)
		{
			var addressViewModel = Mapper.Map<AddressViewModel>(await AddressRepository.GetById(id));
			return addressViewModel;
		}

		[ClaimsAuthorize("Supplier", "Update")]
		[HttpPut("update-address/{id:guid}")]
		public async Task<IActionResult> UpdateAddress(Guid id, AddressViewModel addressViewModel)
		{
			if (id != addressViewModel.Id)
			{
				NotifyError("The informed id is not the same as the one in the query");
				return CustomResponse(addressViewModel);
			}
			if (!ModelState.IsValid)
			{
				return CustomResponse(ModelState);
			}
			await SupplierService.UpdateAddress(Mapper.Map<Address>(addressViewModel));

			return CustomResponse(addressViewModel);
		}
	}
}
