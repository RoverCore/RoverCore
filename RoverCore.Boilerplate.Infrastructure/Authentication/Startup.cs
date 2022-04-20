using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RoverCore.Boilerplate.Domain.DTOs.Authentication;
using RoverCore.Boilerplate.Domain.Entities.Settings;

namespace RoverCore.Boilerplate.Infrastructure.Authentication;

public static class Startup
{
	/// <summary>
	///     Add authentication services required for authentication
	/// </summary>
	public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
	{
		// configure strongly typed settings objects
		var appSettingsSection = configuration.GetSection("JWTSettings");
		services.Configure<JWTSettings>(appSettingsSection);

        var settingsSection = configuration.GetSection("Settings");
        var settings = settingsSection.Get<ApplicationSettings>();

		// configure jwt authentication
		var appSettings = appSettingsSection.Get<JWTSettings>();
		var key = Encoding.ASCII.GetBytes(appSettings.TokenSecret);
		services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie()
			.AddJwtBearer(x =>
			{
				x.RequireHttpsMetadata = false;
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false
				};
			});

        services.ConfigureApplicationCookie(options =>
        {
            options.SlidingExpiration = true;
            options.LoginPath = $"/Identity/Account/Login";
            options.LogoutPath = $"/Identity/Account/Logout";
            options.AccessDeniedPath = $"/Identity/Account/AccessDenied";

			//options.DataProtectionProvider = ???
			if (settings.InactivityTimeout != 0)
            {
                options.ExpireTimeSpan = TimeSpan.FromSeconds(settings.InactivityTimeout);
                options.Cookie.MaxAge = options.ExpireTimeSpan;
            }
        });
    }

	public static void Configure(IApplicationBuilder app, IConfiguration configuration)
	{
        app.UseAuthentication();
	}
}