using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;
using RoverCore.Boilerplate.Infrastructure.Services.Seeder;

namespace RoverCore.Boilerplate.Infrastructure.Persistence.Seeding;

public class ApplicationDbSeed : ISeeder
{
    private readonly ApplicationDbContext _context;

    public ApplicationDbSeed(ApplicationDbContext context)
    {
        _context = context;
    }

    public string GetJson(string seedFile)
    {
        var file = System.IO.File.ReadAllText(Path.Combine("Configuration", "SeedData", seedFile));

        return file;
    }

    /// <summary>
    /// Inserts all the records from a json file into an empty database
    /// </summary>
    /// <typeparam name="TEntity">Model for each record in json file</typeparam>
    /// <param name="jsonFile">JSON encoded array of database records</param>
    /// <param name="dbset">Database dbset to insert into</param>
    /// <param name="preserveOrder">Make sure order is maintained when inserting into database</param>
    public void SeedDatabase<TEntity>(string jsonFile, DbSet<TEntity> dbset, bool preserveOrder = false) where TEntity : class
    {
        var records = JsonConvert.DeserializeObject<List<TEntity>>(GetJson(jsonFile));

        if (records?.Count > 0)
        {
            if (!preserveOrder)
            {
                _context.AddRange(records);
                _context.SaveChanges();
            }
            else
            {
                foreach (var record in records)
                {
                    dbset.Add(record);
                    _context.SaveChanges();
                }
            }
        }
    }

    /// <summary>
    /// Inserts all the records from a json file into a database, adding records
    /// that are not currently in the database if they are found
    /// </summary>
    /// <typeparam name="TEntity">Model for each record in json file</typeparam>
    /// <param name="jsonFile">JSON encoded array of database records</param>
    /// <param name="dbset">Database dbset to insert into</param>
    /// <param name="matchingProperty">Json files will not have primary id keys, so this is used to check to see if a record already exists in table</param>
    public void SeedDatabaseOrUpdate<TEntity>(string jsonFile, DbSet<TEntity> dbset, string? matchingProperty = null) where TEntity : class
    {
        var records = dbset.ToList();
        if (records == null || records.Count == 0)
        {
            SeedDatabase<TEntity>(jsonFile, dbset, true);
        }
        else if (matchingProperty != null)
        {
            var precords = JsonConvert.DeserializeObject<List<TEntity>>(GetJson(jsonFile));
            if (precords != null)
            {
	            foreach (var rec in precords)
	            {
		            var p2 = rec.GetType().GetProperty(matchingProperty)?.GetValue(rec, null);
		            var exists = records.FirstOrDefault(c => c.GetType().GetProperty(matchingProperty)!.GetValue(c, null)!.Equals(p2));

		            if (exists == null)
		            {
			            dbset.Add(rec);
			            _context.SaveChanges();
		            }
	            }
            }
        }


    }

    public Task SeedAsync()
    {
        // SeedDatabaseOrUpdate<Member>(dbContext, "Members.json", dbContext.Member, "LastName");

        return Task.CompletedTask;
    }
}