using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts
{
	public class MultiTenantStoreDbContext : EFCoreStoreDbContext<TenantInfo>
	{
		private readonly IConfiguration _configuration;

		public MultiTenantStoreDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
		{
			_configuration = configuration;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			//options.UseSqlServer(_configuration.GetConnectionString("AppContext"));

			base.OnConfiguring(options);
		}
	}
}
