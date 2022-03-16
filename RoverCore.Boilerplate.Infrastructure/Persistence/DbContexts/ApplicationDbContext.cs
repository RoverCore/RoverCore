using Audit.EntityFramework;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoverCore.Abstractions.Templates;
using RoverCore.Boilerplate.Domain.Entities;
using RoverCore.Boilerplate.Domain.Entities.Audit;
using RoverCore.Boilerplate.Domain.Entities.Identity;
using RoverCore.Boilerplate.Domain.Entities.Serilog;
using RoverCore.Boilerplate.Domain.Entities.Templates;

namespace RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserClaim,
    ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
{
    private readonly DbContextHelper _helper = new DbContextHelper();
    private readonly IAuditDbContext _auditContext;

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        _auditContext = new DefaultAuditContext(this);
        _helper.SetConfig(_auditContext);
    }

    public DbSet<ConfigurationItem> ConfigurationItem { get; set; }
    public DbSet<Member> Member { get; set; }
    public DbSet<Template> Template { get; set; }
    public DbSet<Log> ServiceLog { get; set; }
    public DbSet<AuditLog> AuditLog { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);


        builder.Entity<ApplicationUser>(b =>
        {
            // Each User can have many UserClaims
            b.HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            b.HasMany(e => e.Logins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            b.HasMany(e => e.Tokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        });

        builder.Entity<ApplicationRole>(b =>
        {
            // Each Role can have many entries in the UserRole join table
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // Each Role can have many associated RoleClaims
            b.HasMany(e => e.RoleClaims)
                .WithOne(e => e.Role)
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();
        });
    }

    public override int SaveChanges()
    {
        return _helper.SaveChanges(_auditContext, () => base.SaveChanges());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return await _helper.SaveChangesAsync(_auditContext, () => base.SaveChangesAsync(cancellationToken));
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.