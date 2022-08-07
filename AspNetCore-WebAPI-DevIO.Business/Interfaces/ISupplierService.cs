using AspNetCore_WebAPI_DevIO.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore_WebAPI_DevIO.Business.Interfaces
{
	public interface ISupplierService : IDisposable
	{
		Task<bool> Add(Supplier supplier);
		Task<bool> Update(Supplier supplier);
		Task<bool> Delete(Guid id);
		Task UpdateAddress(Address address);
	}
}
