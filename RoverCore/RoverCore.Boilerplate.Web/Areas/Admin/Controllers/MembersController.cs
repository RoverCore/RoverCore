using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoverCore.BreadCrumbs.Services;
using RoverCore.Boilerplate.Domain.Entities.Identity;
using RoverCore.Boilerplate.Infrastructure.Services;
using RoverCore.Boilerplate.Web.Areas.Identity.Models.AccountViewModels;
using RoverCore.Boilerplate.Web.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;
using RoverCore.Boilerplate.Domain.DTOs.Datatables;
using RoverCore.Boilerplate.Web.Extensions;
using RoverCore.Boilerplate.Infrastructure.Extensions;
using RoverCore.Boilerplate.Domain.Entities;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;

namespace RoverCore.Boilerplate.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MembersController : BaseController<MembersController>
    {
        private readonly ApplicationDbContext _context;

        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Members
        public IActionResult Index()
        {
            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .Then("Manage Member");            
            
            return View();
        }

        // GET: Admin/Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
                .ThenAction("Manage Member", "Index", "MembersController", new { Area = "Admin" })
                .Then("Member Details");            

            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Admin/Members/Create
        public IActionResult Create()
        {
            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
                .ThenAction("Manage Member", "Index", "MembersController", new { Area = "Admin" })
                .Then("Create Member");     

            return View();
        }

        // POST: Admin/Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,FirstName,LastName,Email")] Member member)
        {
            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .ThenAction("Manage Member", "Index", "MembersController", new { Area = "Admin" })
            .Then("Create Member");     

            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                
                _toast.Success("Created successfully.");
                
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Admin/Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .ThenAction("Manage Member", "Index", "MembersController", new { Area = "Admin" })
            .Then("Edit Member");     

            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            

            return View(member);
        }

        // POST: Admin/Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,FirstName,LastName,Email")] Member member)
        {
            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .ThenAction("Manage Member", "Index", "MembersController", new { Area = "Admin" })
            .Then("Edit Member");  
        
            if (id != member.MemberId)
            {
                return NotFound();
            }
            
            Member model = await _context.Member.FindAsync(id);

            model.FirstName = member.FirstName;
            model.LastName = member.LastName;
            model.Email = member.Email;

            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    _toast.Success("Updated successfully.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.MemberId))
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
            return View(member);
        }

        // GET: Admin/Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .ThenAction("Manage Member", "Index", "MembersController", new { Area = "Admin" })
            .Then("Delete Member");  

            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Admin/Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Member.FindAsync(id);
            _context.Member.Remove(member);
            await _context.SaveChangesAsync();
            
            _toast.Success("Member deleted successfully");

            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.MemberId == id);
        }

        private IQueryable<Member> GetMemberQueryable()
        {
            return _context.Member.AsQueryable();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> GetMember(DtRequest request)
        {
            try
            {
                var sortColumn = request.Columns[request.Order[0].Column].Name;
                var sortColumnDirection = request.Order[0].Dir;
                var searchValue = request.Search.Value;

                int recordsTotal = 0;
                var records = GetMemberQueryable();

                sortColumn = string.IsNullOrEmpty(sortColumn) ? "MemberId" : sortColumn.Replace(" ", "");
                sortColumnDirection = string.IsNullOrEmpty(sortColumnDirection) ? "asc" : sortColumnDirection;

                if (!string.IsNullOrEmpty(searchValue))
                {
                    records = records.Where(m => m.FirstName.ToString().Contains(searchValue)
                                || m.LastName.ToString().Contains(searchValue)
                                || m.Email.ToString().Contains(searchValue));
                }
                
                records = sortColumnDirection == "asc" ? records.OrderBy(sortColumn) : records.OrderByDescending(sortColumn);

                var recordsList = await records.ToListAsync();

                recordsTotal = recordsList.Count();
                var data = recordsList.Skip(request.Start).Take(request.Length)
	                .Select(x => new
                    {
                        memberId = x.MemberId,
                        firstName = x.FirstName,
                        lastName = x.LastName,
                        email = x.Email,
	                    Options = ""
	                }).ToList();

                var jsonData = new { 
                    draw = request.Draw, 
                    recordsFiltered = recordsTotal, 
                    recordsTotal = recordsTotal, 
                    data = data 
                };

                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
