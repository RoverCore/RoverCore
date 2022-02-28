using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoverCore.Boilerplate.Infrastructure.Common.Seeder.Services;

namespace RoverCore.Boilerplate.Infrastructure.Common.Finbuckle.Seeding;

public class DefaultTenantSeed : ISeeder
{
    private readonly IMultiTenantStore<TenantInfo> _tenantStore;
    private readonly IConfiguration _configuration;

    public DefaultTenantSeed(IMultiTenantStore<TenantInfo> tenantStore, IConfiguration configuration)
    {
	    _tenantStore = tenantStore;
        _configuration = configuration;
    }

    public void CreateTenants()
    {
	    _tenantStore.TryAddAsync(new TenantInfo
		    { Id = "a5883f2-38ee-4993-8abc-e63fe3f9daf2", Identifier = "default-tenant", Name = "Default Tenant", ConnectionString = _configuration.GetConnectionString("AppContext") });
    }

    public Task SeedAsync()
    {
        CreateTenants();

        return Task.CompletedTask;
    }
}