using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using AspNetCore_WebAPI_DevIO.Business.Models;
using AspNetCore_WebAPI_DevIO.Business.Models.Validations;
using System;
using System.Threading.Tasks;

namespace AspNetCore_WebAPI_DevIO.Business.Services
{
	public class ProductService : BaseService, IProductService
	{
		private readonly IProductRepository ProductRepository;
		private readonly IUser User;

		public ProductService(IProductRepository productRepository, IUser user, INotifier notifier) : base(notifier)
		{
			ProductRepository = productRepository;
			User = user;
		}

		public async Task Add(Product product)
		{
			if (!ExecuteValidation(new ProductValidation(), product))
			{
				return;
			}

			await ProductRepository.Add(product);
		}

		public async Task Delete(Guid id) => await ProductRepository.Delete(id);

		public async Task Update(Product product)
		{
			if (!ExecuteValidation(new ProductValidation(), product))
			{
				return;
			}

			await ProductRepository.Update(product);
		}

		public void Dispose()
		{
			ProductRepository?.Dispose();
		}
	}
}
