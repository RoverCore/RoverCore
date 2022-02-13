﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
using RoverCore.Boilerplate.Domain.Entities.Templates;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;
using RoverCore.Boilerplate.Infrastructure.Services.Templates;

namespace RoverCore.Boilerplate.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TemplateController : BaseController<TemplateController>
    {
        private const string createBindingFields = "Id,Slug,Name,Description,Body";
        private const string editBindingFields = "Id,Slug,Name,Description,Body";
        private const string areaTitle = "Admin";

        private readonly TemplateService _templateService;

        public TemplateController(TemplateService templateService)
        {
            _templateService = templateService;
        }

        // GET: Admin/Template
        public IActionResult Index()
        {
            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .Then("Manage Email Templates");            
            
            return View();
        }

        // GET: Admin/Template/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["AreaTitle"] = areaTitle;
            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
                .ThenAction("Manage Email Templates", "Index", "Template", new { Area = "Admin" })
                .Then("Template Details");            

            if (id == null)
            {
                return NotFound();
            }

            var template = await _templateService.FindTemplateById((int)id);

            if (template == null)
            {
                return NotFound();
            }

            return View(template);
        }

        // GET: Admin/Template/Create
        public IActionResult Create()
        {
            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
                .ThenAction("Manage Email Templates", "Index", "Template", new { Area = "Admin" })
                .Then("Create Template");     

            return View();
        }

        // POST: Admin/Template/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(createBindingFields)] Template template)
        {
            ViewData["AreaTitle"] = areaTitle;

            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .ThenAction("Manage Template", "Index", "TemplateController", new { Area = "Admin" })
            .Then("Create Template");     
            
            // Remove validation errors from fields that aren't in the binding field list
            ModelState.Scrub(createBindingFields);           

            if (ModelState.IsValid)
            {
                await _templateService.CreateTemplate(template);
                
                _toast.Success("Created successfully.");
                
                return RedirectToAction(nameof(Index));
            }
            return View(template);
        }

        // GET: Admin/Template/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["AreaTitle"] = areaTitle;

            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .ThenAction("Manage Email Templates", "Index", "Template", new { Area = "Admin" })
            .Then("Edit Template");     

            if (id == null)
            {
                return NotFound();
            }

            var template = await _templateService.FindTemplateById((int)id);
            if (template == null)
            {
                return NotFound();
            }
            

            return View(template);
        }

        // POST: Admin/Template/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(editBindingFields)] Template template)
        {
            ViewData["AreaTitle"] = areaTitle;

            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .ThenAction("Manage Email Templates", "Index", "Template", new { Area = "Admin" })
            .Then("Edit Template");  
        
            if (id != template.Id)
            {
                return NotFound();
            }

            // Remove validation errors from fields that aren't in the binding field list
            ModelState.Scrub(editBindingFields);           

            if (ModelState.IsValid)
            {
                try
                {
	                await _templateService.UpdateTemplate(template);
                    
                    _toast.Success("Updated successfully.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_templateService.TemplateExists(template.Id))
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
            return View(template);
        }

        // GET: Admin/Template/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["AreaTitle"] = areaTitle;

            _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .ThenAction("Manage Email Templates", "Index", "Template", new { Area = "Admin" })
            .Then("Delete Template");  

            if (id == null)
            {
                return NotFound();
            }

            var template = await _templateService.FindTemplateById((int)id);

            if (template == null)
            {
                return NotFound();
            }

            return View(template);
        }

        // POST: Admin/Template/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
			await _templateService.DeleteTemplate((int)id);
            
            _toast.Success("Template deleted successfully");

            return RedirectToAction(nameof(Index));
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> GetTemplate(DtRequest request)
        {
            try
            {
                var sortColumn = request.Columns[request.Order[0].Column].Name;
                var sortColumnDirection = request.Order[0].Dir;
                var searchValue = request.Search.Value;

                int recordsTotal = 0;
                var records = _templateService.GetTemplateQueryable();

                sortColumn = string.IsNullOrEmpty(sortColumn) ? "Id" : sortColumn.Replace(" ", "");
                sortColumnDirection = string.IsNullOrEmpty(sortColumnDirection) ? "asc" : sortColumnDirection;

                if (!string.IsNullOrEmpty(searchValue))
                {
                    records = records.Where(m => m.Slug.Contains(searchValue)
                                || m.Name.Contains(searchValue)
                                || m.Description.Contains(searchValue)
                                || m.Body.Contains(searchValue));
                }
                
                records = sortColumnDirection == "asc" ? records.OrderBy(sortColumn) : records.OrderByDescending(sortColumn);

                var recordsList = await records.ToListAsync();

                recordsTotal = recordsList.Count();
                var data = recordsList.Skip(request.Start).Take(request.Length)
	                .Select(x => new
                    {
                        Options = "",
                        id = x.Id,
                        slug = x.Slug,
                        name = x.Name,
                        updated = x.Updated.ToString("yyyy-MM-dd HH:mm")
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
                _logger.LogError(ex, "Error generating Template index json");
            }
            
            return StatusCode(500);
        }

    }
}