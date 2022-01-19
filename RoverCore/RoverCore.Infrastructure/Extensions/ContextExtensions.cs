using Microsoft.EntityFrameworkCore;
using RoverCore.Infrastructure.Persistence.DbContexts;

namespace RoverCore.Infrastructure.Extensions;

public static class ContextExtensions
{
    public static void AddOrUpdate(this ApplicationDbContext ctx, object entity)
    {
        var entry = ctx.Entry(entity);
        switch (entry.State)
        {
            case EntityState.Detached:
                ctx.Add(entity);
                break;
            case EntityState.Modified:
                ctx.Update(entity);
                break;
            case EntityState.Added:
                ctx.Add(entity);
                break;
            case EntityState.Unchanged:
                // item already in db no need to do anything  
                break;

            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }
}