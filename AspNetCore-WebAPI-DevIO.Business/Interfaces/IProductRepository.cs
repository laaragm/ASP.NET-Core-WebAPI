using AspNetCore_WebAPI_DevIO.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore_WebAPI_DevIO.Business.Interfaces
{
	public interface IProductRepository : IRepository<Product>
	{
		Task<IEnumerable<Product>> GetProductsBySupplier(Guid supplierId);
		Task<IEnumerable<Product>> GetProductsSuppliers();
		Task<Product> GetProductSupplier(Guid id);
	}
}
