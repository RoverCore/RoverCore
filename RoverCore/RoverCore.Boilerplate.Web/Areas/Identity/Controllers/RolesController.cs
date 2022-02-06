using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoverCore.Boilerplate.Domain.DTOs.Datatables;
using RoverCore.Boilerplate.Domain.Entities.Identity;
using RoverCore.Boilerplate.Infrastructure.Extensions;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;
using RoverCore.Boilerplate.Web.Controllers;
using RoverCore.BreadCrumbs.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RoverCore.Boilerplate.Web.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize(Roles = "Admin")]
    public class RolesController : BaseController<RolesController>
    {
        private const string createBindingFields = "Name";
        private const string editBindingFields = "Id,Name,NormalizedName,ConcurrencyStamp";
        private const string areaTitle = "Identity";

        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Identity/Roles
        public IActionResult Index()
        {
            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .Then("Manage Roles");

            return View();
        }

        // GET: Identity/Roles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            ViewData["AreaTitle"] = areaTitle;
            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
                .ThenAction("Manage Roles", "Index", "Roles", new { Area = "Identity" })
                .Then("Role Details");

            if (id == null)
            {
                return NotFound();
            }

            var applicationRole = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationRole == null)
            {
                return NotFound();
            }

            return View(applicationRole);
        }

        // GET: Identity/Roles/Create
        public IActionResult Create()
        {
            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
                .ThenAction("Manage Roles", "Index", "Roles", new { Area = "Identity" })
                .Then("Create Role");

            return View();
        }

        // POST: Identity/Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(createBindingFields)] ApplicationRole applicationRole)
        {
            ViewData["AreaTitle"] = areaTitle;

            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .ThenAction("Manage Roles", "Index", "RolesController", new { Area = "Identity" })
            .Then("Create Role");

            // Remove validation errors from fields that aren't in the binding field list
            ModelState.Scrub(createBindingFields);

            if (ModelState.IsValid)
            {
                applicationRole.Id = Guid.NewGuid().ToString();
                applicationRole.NormalizedName = applicationRole.Name.ToUpper();
                applicationRole.ConcurrencyStamp = Guid.NewGuid().ToString();

                _context.Add(applicationRole);
                await _context.SaveChangesAsync();

                _toast.Success("Created successfully.");

                return RedirectToAction(nameof(Index));
            }
            return View(applicationRole);
        }

        // GET: Identity/Roles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            ViewData["AreaTitle"] = areaTitle;

            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .ThenAction("Manage Roles", "Index", "Roles", new { Area = "Identity" })
            .Then("Edit Role");

            if (id == null)
            {
                return NotFound();
            }

            var applicationRole = await _context.Roles.FindAsync(id);
            if (applicationRole == null)
            {
                return NotFound();
            }


            return View(applicationRole);
        }

        // POST: Identity/Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind(editBindingFields)] ApplicationRole applicationRole)
        {
            ViewData["AreaTitle"] = areaTitle;

            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .ThenAction("Manage Roles", "Index", "Roles", new { Area = "Identity" })
            .Then("Edit Role");

            if (id != applicationRole.Id)
            {
                return NotFound();
            }

            ApplicationRole model = await _context.Roles.FindAsync(id);

            model.Name = applicationRole.Name;
            model.NormalizedName = applicationRole.NormalizedName;
            model.ConcurrencyStamp = applicationRole.ConcurrencyStamp;
            // Remove validation errors from fields that aren't in the binding field list
            ModelState.Scrub(editBindingFields);

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    _toast.Success("Updated successfully.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationRoleExists(applicationRole.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(applicationRole);
        }

        // GET: Identity/Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            ViewData["AreaTitle"] = areaTitle;

            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .ThenAction("Manage Roles", "Index", "Roles", new { Area = "Identity" })
            .Then("Delete Role");

            if (id == null)
            {
                return NotFound();
            }

            var applicationRole = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationRole == null)
            {
                return NotFound();
            }

            return View(applicationRole);
        }

        // POST: Identity/Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationRole = await _context.Roles.FindAsync(id);
            _context.Roles.Remove(applicationRole);
            await _context.SaveChangesAsync();

            _toast.Success("Role deleted successfully");

            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationRoleExists(string id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }

        private IQueryable<ApplicationRole> GetRolesQueryable()
        {
            return _context.Roles.AsQueryable();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetRoles(DtRequest request)
        {
            try
            {
                var sortColumn = request.Columns[request.Order[0].Column].Name;
                var sortColumnDirection = request.Order[0].Dir;
                var searchValue = request.Search.Value;

                int recordsTotal = 0;
                var records = GetRolesQueryable();

                sortColumn = string.IsNullOrEmpty(sortColumn) ? "Id" : sortColumn.Replace(" ", "");
                sortColumnDirection = string.IsNullOrEmpty(sortColumnDirection) ? "asc" : sortColumnDirection;

                if (!string.IsNullOrEmpty(searchValue))
                {
                    records = records.Where(m => m.Name.Contains(searchValue)
                                || m.NormalizedName.Contains(searchValue)
                                || m.ConcurrencyStamp.Contains(searchValue));
                }

                records = sortColumnDirection == "asc" ? records.OrderBy(sortColumn) : records.OrderByDescending(sortColumn);

                var recordsList = await records.ToListAsync();

                recordsTotal = recordsList.Count();
                var data = recordsList.Skip(request.Start).Take(request.Length)
                    .Select(x => new
                    {
                        id = x.Id,
                        name = x.Name,
                        normalizedName = x.NormalizedName,
                        concurrencyStamp = x.ConcurrencyStamp,
                        Options = ""
                    }).ToList();

                var jsonData = new
                {
                    draw = request.Draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = data
                };

                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Roles index json");
            }

            return StatusCode(500);
        }

    }
}
