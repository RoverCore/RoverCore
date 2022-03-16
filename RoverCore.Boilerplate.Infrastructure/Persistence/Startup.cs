using Audit.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoverCore.Boilerplate.Domain.Entities.Audit;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;

namespace RoverCore.Boilerplate.Infrastructure.Persistence;

public static class Startup
{
	/// <summary>
	///     Adds all Entity Framework database contexts to the service collection
	/// </summary>
	public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ApplicationDbContext>(options =>
		{
			options.UseSqlServer(configuration.GetConnectionString("AppContext"),
				x => x.MigrationsAssembly("RoverCore.Boilerplate.Infrastructure"));
		}, optionsLifetime: ServiceLifetime.Singleton, contextLifetime: ServiceLifetime.Scoped);

		services.AddDbContextFactory<ApplicationDbContext>(options =>
		{
			options.UseSqlServer(configuration.GetConnectionString("AppContext"),
				x => x.MigrationsAssembly("RoverCore.Boilerplate.Infrastructure"));
		});

        Audit.EntityFramework.Configuration.Setup()
            .ForContext<ApplicationDbContext>(config => config
                .IncludeEntityObjects()
                .AuditEventType("{context}:{database}"))
                .UseOptIn();

		Audit.Core.Configuration.Setup()
			.UseEntityFramework(_ => _
                .UseDbContext<ApplicationDbContext>()
                .AuditTypeMapper(t => typeof(AuditLog))
                .AuditEntityAction<AuditLog>((ev, entry, entity) =>
                {
                    entity.TableName = entry.Table;
                    entity.AuditData = entry.ToJson();
                    entity.EntityType = entry.EntityType.Name;
                    entity.AuditDate = DateTime.UtcNow;
                    entity.AuditUser = Environment.UserName;
                    entity.TablePK = entry.PrimaryKey.First().Value.ToString();
                })
                .IgnoreMatchedProperties(true));
	}

	public static void Configure(IApplicationBuilder app, IConfiguration configuration)
	{
	}
}

