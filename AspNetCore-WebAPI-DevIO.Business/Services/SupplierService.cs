using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using AspNetCore_WebAPI_DevIO.Business.Models;
using AspNetCore_WebAPI_DevIO.Business.Models.Validations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore_WebAPI_DevIO.Business.Services
{
	public class SupplierService : BaseService, ISupplierService
	{
		private readonly ISupplierRepository SupplierRepository;
		private readonly IAddressRepository AddressRepository;

		public SupplierService(ISupplierRepository supplierRepository, IAddressRepository addressRepository, INotifier notifier) : base(notifier)
		{
			SupplierRepository = supplierRepository;
			AddressRepository = addressRepository;
		}

		public async Task<bool> Add(Supplier supplier)
		{
			if (!ExecuteValidation(new SupplierValidation(), supplier) || !ExecuteValidation(new AddressValidation(), supplier.Address))
			{
				return false;
			}

			if (SupplierRepository.Search(f => f.Document == supplier.Document).Result.Any())
			{
				Notify("A supplier with the informed document already exists.");
				return false;
			}

			await SupplierRepository.Add(supplier);

			return true;
		}

		public async Task<bool> Delete(Guid id)
		{
			if (SupplierRepository.GetSuppliersProductsAddress(id).Result.Products.Any())
			{
				Notify("The supplier has registered products.");
				return false;
			}

			var address = await AddressRepository.GetAddressBySupplier(id);
			if (address != null)
			{
				await AddressRepository.Delete(address.Id);
			}

			await SupplierRepository.Delete(id);
			return true;
		}

		public void Dispose()
		{
			SupplierRepository?.Dispose();
			AddressRepository?.Dispose();
		}

		public async Task<bool> Update(Supplier supplier)
		{
			if (!ExecuteValidation(new SupplierValidation(), supplier))
			{
				return false;
			}

			if (SupplierRepository.Search(f => f.Document == supplier.Document && f.Id != supplier.Id).Result.Any())
			{
				Notify("A supplier with the informed document already exists.");
				return false;
			}

			await SupplierRepository.Update(supplier);
			return true;
		}

		public async Task UpdateAddress(Address address)
		{
			if (!ExecuteValidation(new AddressValidation(), address))
			{
				return;
			}

			await AddressRepository.Update(address);
		}
	}
}
