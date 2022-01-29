using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoverCore.Domain.Entities.Identity;
using RoverCore.Infrastructure.Persistence.DbContexts;
using RoverCore.Infrastructure.Services;
using RoverCore.Boilerplate.Web.Areas.Identity.Models.AccountViewModels;
using RoverCore.Boilerplate.Web.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;
using RoverCore.Boilerplate.Web.Extensions;

namespace RoverCore.Boilerplate.Web.Areas.Identity.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Area("Identity")]
[Authorize(Roles = "Admin")]
public class UsersController : BaseController
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

    public async Task<IActionResult> Index()
    {
        var users = await _context.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).OrderBy(x => x.LastName).ToListAsync();

        var viewModel = users.Select(x => new UserViewModel
        {
            Id = x.Id,
            Email = x.Email,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Roles = x.UserRoles.Select(ur => ur.Role.Name).ToList()
        }).ToList();

        return View(viewModel);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Email,FirstName,LastName,Roles,Password,ConfirmPassword")] UserViewModel viewModel)
    {
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
        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
        return View(viewModel);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _context.Users.FindAsync(id);
        var roles = await _userManager.GetRolesAsync(user);

        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");

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

        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");

        return View(viewModel);
    }

    public async Task<IActionResult> Details(string id)
    {
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
}