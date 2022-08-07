using AspNetCore_WebAPI_DevIO.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore_WebAPI_DevIO.Business.Interfaces
{
	public interface ISupplierRepository : IRepository<Supplier>
	{
		Task<Supplier> GetSuppliersAddress(Guid id);
		Task<Supplier> GetSuppliersProductsAddress(Guid id);
	}
}
