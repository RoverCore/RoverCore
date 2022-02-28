using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Stores;
using Microsoft.EntityFrameworkCore;

namespace RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts
{
	public class MultiTenantStoreDbContext : EFCoreStoreDbContext<TenantInfo>
	{
		public MultiTenantStoreDbContext(DbContextOptions options) : base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
		}
	}
}
