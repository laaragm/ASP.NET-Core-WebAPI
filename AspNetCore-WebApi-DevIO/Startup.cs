using AspNetCore_WebApi_DevIO.Configuration;
using AspNetCore_WebAPI_DevIO.Data.Context;
using AutoMapper;
using DevIO.Api.Configurations;
using Mailing.Domain;
using Mailing.Domain.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore_WebApi_DevIO
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;

			// Mailing config
			var mailingSection = Configuration.GetSection("MailSettings");
			MailingConfig = new MailingConfig(mailingSection["MailingUserName"], mailingSection["MailingApiKey"], mailingSection["MailingHost"], int.Parse(mailingSection["MailingPort"]), mailingSection["MailingEmail"], mailingSection["MailingPassword"], bool.Parse(mailingSection["MailingEnableSSL"]));
		}

		public IConfiguration Configuration { get; }
		public IMailingConfig MailingConfig { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<DatabaseContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnectionString"));
			});

			services.AddIdentityConfig(Configuration);

			services.AddSingleton(MailingConfig);

			services.AddAutoMapper(typeof(Startup)); // When we add the 'typeof(Startup' it basically says that it'll resolve everything from this particular assembly

			services.AddApiConfig();

			services.AddControllers();

			services.ResolveDependencies();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseApiConfig(env);
		}
	}
}
