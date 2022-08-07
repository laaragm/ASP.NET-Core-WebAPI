using AspNetCore_WebAPI_DevIO.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore_WebAPI_DevIO.Business.Interfaces
{
	public interface IProductService
	{
		Task Add(Product product);
		Task Update(Product product);
		Task Delete(Guid id);
	}
}
