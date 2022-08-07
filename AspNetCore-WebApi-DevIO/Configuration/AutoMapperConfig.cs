using AspNetCore_WebApi_DevIO.ViewModels;
using AspNetCore_WebAPI_DevIO.Business.Models;
using AutoMapper;

namespace AspNetCore_WebApi_DevIO.Configuration
{
	// In the configuration cycle, the Auto Mapper will basically look at the Startup assembly and resolve all the mappings that implements the Profile
	public class AutoMapperConfig : Profile
	{
		public AutoMapperConfig()
		{
			CreateMap<Supplier, SupplierViewModel>().ReverseMap();
			CreateMap<Address, AddressViewModel>().ReverseMap();
			CreateMap<ProductViewModel, Product>().ReverseMap();
			CreateMap<Product, ProductViewModel>()
				.ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name));
		}
	}
}
