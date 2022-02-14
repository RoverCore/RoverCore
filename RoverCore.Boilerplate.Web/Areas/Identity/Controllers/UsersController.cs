using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoverCore.Boilerplate.Domain.DTOs.Datatables;
using RoverCore.Boilerplate.Domain.Entities.Identity;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;
using RoverCore.Boilerplate.Web.Areas.Identity.Models.AccountViewModels;
using RoverCore.Boilerplate.Web.Controllers;
using RoverCore.Boilerplate.Web.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using RoverCore.Boilerplate.Infrastructure.Common;
using RoverCore.Boilerplate.Infrastructure.Persistence.Extensions;
using RoverCore.BreadCrumbs.Services;

namespace RoverCore.Boilerplate.Web.Areas.Identity.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Area("Identity")]
[Authorize(Roles = "Admin")]
public class UsersController : BaseController<UsersController>
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IEmailSender _emailSender;

    public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IEmailSender emailSender)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
    }

    public IActionResult Index()
    {
	    _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
		    .Then("Manage Users");

        return View(new UserViewModel());
    }

    public async Task<IActionResult> Create()
    {
	    _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
		    .ThenAction("Manage Users", "Index", "Users", new { Area = "Identity" })
		    .Then("Create User");

        ViewBag.Roles = new SelectList(await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync(), "Name", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Email,FirstName,LastName,Roles,Password,ConfirmPassword")] UserViewModel viewModel)
    {
	    _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
		    .ThenAction("Manage Users", "Index", "Users", new { Area = "Identity" })
		    .Then("Create User");

        if (string.IsNullOrEmpty(viewModel.Password))
        {
            ModelState.AddModelError("Password", "Password is required when creating a user");
        }

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = viewModel.Email,
                Email = viewModel.Email,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName
            };

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, viewModel.Password);

            // create user
            await _userManager.CreateAsync(user);

            // assign new roles
            await _userManager.AddToRolesAsync(user, viewModel.Roles);

            // send confirmation email
            var confirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.EmailConfirmationLink(user.Id, confirmationCode, Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(viewModel.Email, confirmationLink);

            return RedirectToAction(nameof(Index));
        }
        ViewBag.Roles = new SelectList(await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync(), "Name", "Name");
        return View(viewModel);
    }

    public async Task<IActionResult> Edit(string id)
    {
	    _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
		    .ThenAction("Manage Users", "Index", "Users", new { Area = "Identity" })
		    .Then("Edit User");

        var user = await _context.Users.FindAsync(id);
        var roles = await _userManager.GetRolesAsync(user);

        ViewBag.Roles = new SelectList(await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync(), "Name", "Name");

        var viewModel = new UserViewModel
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,Email,FirstName,LastName,Roles,Password,ConfirmPassword")] UserViewModel viewModel)
    {
	    _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
		    .ThenAction("Manage Users", "Index", "Users", new { Area = "Identity" })
		    .Then("Edit User");

        if (id != viewModel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                user.Email = viewModel.Email;
                user.FirstName = viewModel.FirstName;
                user.LastName = viewModel.LastName;

                if (!string.IsNullOrEmpty(viewModel.Password))
                {
                    // change the password
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, viewModel.Password);
                }

                // update user
                _context.Update(user);
                await _context.SaveChangesAsync();

                // reset user roles
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);

                // assign new role
                await _userManager.AddToRolesAsync(user, viewModel.Roles);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(viewModel.Id))
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

        ViewBag.Roles = new SelectList(await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync(), "Name", "Name");

        return View(viewModel);
    }

    public async Task<IActionResult> Details(string id)
    {
	    _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
		    .ThenAction("Manage Users", "Index", "Users", new { Area = "Identity" })
		    .Then("User Details");

        var user = await _context.Users.FindAsync(id);
        var roles = await _userManager.GetRolesAsync(user);

        var viewModel = new UserViewModel
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Delete(string id)
    {
	    _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
		    .ThenAction("Manage Users", "Index", "Users", new { Area = "Identity" })
		    .Then("Delete User");

        var user = await _context.Users.FindAsync(id);
        var roles = await _userManager.GetRolesAsync(user);

        var viewModel = new UserViewModel
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        };

        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        await _userManager.DeleteAsync(user);
        return RedirectToAction(nameof(Index));
    }

    private bool UserExists(string id)
    {
        return _context.Users.Any(x => x.Id == id);
    }

    private IQueryable<UserViewModel> GetUsersAsync()
    {
        return _context.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .Select(
            x => new UserViewModel()
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Roles = x.UserRoles.Select(ur => ur.Role.Name).ToList()
            }).AsQueryable();

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetUsers(DtRequest request)
    {
        try
        {
            var sortColumn = request.Columns[request.Order[0].Column].Name;
            var sortColumnDirection = request.Order[0].Dir;
            var searchValue = request.Search.Value;

            int recordsTotal = 0;
            var users = GetUsersAsync();

            sortColumn = string.IsNullOrEmpty(sortColumn) ? "LastName" : sortColumn.Replace(" ", "");
            sortColumnDirection = string.IsNullOrEmpty(sortColumnDirection) ? "asc" : sortColumnDirection;

            if (!string.IsNullOrEmpty(searchValue))
            {
                users = users.Where(m => m.FirstName.Contains(searchValue)
                                            || m.LastName.Contains(searchValue)
                                            || m.Email.Contains(searchValue)
                                            || m.Roles.Contains(searchValue));
            }

            switch (sortColumn)
            {
                case "Roles":

                    users = sortColumnDirection == "asc" ? users.OrderBy(x => string.Join(", ", x.Roles)) :
                        users.OrderByDescending(x => string.Join(", ", x.Roles));

                    break;

                default:

                    users = sortColumnDirection == "asc" ?
                        users.OrderBy(sortColumn) :
                        users.OrderByDescending(sortColumn);

                    break;

            }

            var usersList = await users.ToListAsync();

            recordsTotal = usersList.Count();
            var data = usersList.Skip(request.Start).Take(request.Length)
                .Select(x => new
                {
                    Options = "",
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Roles = String.Join(", ", x.Roles),
                    Email = x.Email
                }).ToList();

            var jsonData = new { draw = request.Draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
            return Ok(jsonData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating user index json");
        }

        return StatusCode(500);
    }
}