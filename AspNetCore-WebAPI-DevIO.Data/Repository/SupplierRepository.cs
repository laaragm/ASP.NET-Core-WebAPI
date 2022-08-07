using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using AspNetCore_WebAPI_DevIO.Business.Models;
using AspNetCore_WebAPI_DevIO.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore_WebAPI_DevIO.Data.Repository
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(DatabaseContext context) : base(context) 
        {
        }

		public async Task<Supplier> GetSuppliersAddress(Guid id)
		{
			return await DatabaseContext.Suppliers.AsNoTracking()
				.Include(c => c.Address)
				.FirstOrDefaultAsync(c => c.Id == id);
		}

		public async Task<Supplier> GetSuppliersProductsAddress(Guid id)
		{
			return await DatabaseContext.Suppliers.AsNoTracking()
				.Include(c => c.Products)
				.Include(c => c.Address)
				.FirstOrDefaultAsync(c => c.Id == id);
		}
	}
}
