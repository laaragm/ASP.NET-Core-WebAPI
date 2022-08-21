using AspNetCore_WebApi_DevIO.Extensions;
using AspNetCore_WebApi_DevIO.Services;
using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using AspNetCore_WebAPI_DevIO.Business.Notifications;
using AspNetCore_WebAPI_DevIO.Business.Services;
using AspNetCore_WebAPI_DevIO.Data.Context;
using AspNetCore_WebAPI_DevIO.Data.Repository;
using Mailing.Services;
using Mailing.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCore_WebApi_DevIO.Configuration
{
	public static class DependencyInjectionConfig
	{
		public static IServiceCollection ResolveDependencies(this IServiceCollection services)
		{
			services.AddScoped<DatabaseContext>();
			services.AddScoped<ISupplierRepository, SupplierRepository>();
			services.AddScoped<IProductRepository, ProductRepository>();
			services.AddScoped<IAddressRepository, AddressRepository>();

			services.AddScoped<ISupplierService, SupplierService>();
			services.AddScoped<IProductService, ProductService>();
			services.AddScoped<INotifier, Notifier>();
			services.AddScoped<ITransmissionService, TransmissionService>();
			services.AddScoped<AuthenticationService>();

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<IUser, AspNetUser>();

			services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

			return services;
		}
	}
}
