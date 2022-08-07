using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using AspNetCore_WebAPI_DevIO.Business.Models;
using AspNetCore_WebAPI_DevIO.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore_WebAPI_DevIO.Data.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
	{
        public ProductRepository(DatabaseContext context) : base(context) 
        {
        }

		public async Task<IEnumerable<Product>> GetProductsBySupplier(Guid supplierId)
			=> await Search(p => p.SupplierId == supplierId);

		public async Task<IEnumerable<Product>> GetProductsSuppliers()
		{
			return await DatabaseContext.Products.AsNoTracking()
				.Include(f => f.Supplier)
				.OrderBy(p => p.Name).ToListAsync();
		}

		public async Task<Product> GetProductSupplier(Guid id)
		{
			return await DatabaseContext.Products.AsNoTracking()
				.Include(f => f.Supplier)
				.FirstOrDefaultAsync(p => p.Id == id);
		}
	}
}
