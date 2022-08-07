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
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(DatabaseContext context) : base(context) 
        {
        }

        public async Task<Address> GetAddressBySupplier(Guid fornecedorId)
            => await DatabaseContext.Addresses.AsNoTracking().FirstOrDefaultAsync(f => f.SupplierId == fornecedorId);
    }
}
