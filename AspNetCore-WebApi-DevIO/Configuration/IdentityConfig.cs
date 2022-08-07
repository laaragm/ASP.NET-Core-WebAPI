using AspNetCore_WebApi_DevIO.Data;
using AspNetCore_WebApi_DevIO.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AspNetCore_WebApi_DevIO.Configuration
{
    public static class IdentityConfig
	{
        public static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
        {
            // We'll use a different context for Identity to use Entity Framework to manage the users of the application
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("DatabaseConnectionString"));
			});

			services.AddDefaultIdentity<IdentityUser>()
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			// JWT Configuration
			var appSettingsSection = configuration.GetSection("AppSettings");
			services.Configure<AppSettings>(appSettingsSection);

			var appSettings = appSettingsSection.Get<AppSettings>();
			var key = Encoding.ASCII.GetBytes(appSettings.Secret);

			services
				.AddAuthentication(x =>
				{
					x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(x =>
				{
					x.RequireHttpsMetadata = true; // Set this as true if you're only using https
					x.SaveToken = true; // True means tokens are cached in the server for validation
					x.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true, // Validate if the issuer which is in the token is the same as the configured one - this validation is made based on the key
						IssuerSigningKey = new SymmetricSecurityKey(key),
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidAudience = appSettings.Audience,
						ValidIssuer = appSettings.Issuer
					};
				});

			return services;
		}
	}
}
