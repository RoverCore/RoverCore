using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoverCore.BreadCrumbs.Services;
using RoverCore.Datatables.DTOs;
using RoverCore.Datatables.Extensions;
using RoverCore.Boilerplate.Web.Controllers;
using RoverCore.Boilerplate.Infrastructure.Common.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using RoverCore.Boilerplate.Domain.Entities.Serilog;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;

namespace RoverCore.Boilerplate.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class LogsController : BaseController<LogsController>
{
	public class ServiceLogIndexViewModel 
	{
		[Key]            
	    public int Id { get; set; }
	    public string Message { get; set; }
	    public string Level { get; set; }
	    public DateTime TimeStamp { get; set; }
	}

    private const string areaTitle = "Admin";

    private readonly ApplicationDbContext _context;

    public LogsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Logs
    public IActionResult Index()
    {
        _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
			.Then("Service Logs");       
		
		// Fetch descriptive data from the index dto to build the datatables index
		var metadata = DatatableExtensions.GetDtMetadata<ServiceLogIndexViewModel>();
		
		return View(metadata);
   }

    // GET: Admin/Logs/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        ViewData["AreaTitle"] = areaTitle;
        _breadcrumbs.StartAtAction("Dashboard", "Index", "Home", new { Area = "Dashboard" })
            .ThenAction("Service Logs", "Index", "Logs", new { Area = "Admin" })
            .Then("Log Details");            

        if (id == null)
        {
            return NotFound();
        }

        var log = await _context.ServiceLog
            .FirstOrDefaultAsync(m => m.Id == id);
        if (log == null)
        {
            return NotFound();
        }

        try
        {
            log.Properties = PrettyXml(log.Properties);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unable to parse log properties for log id {log.Id}");
        }

        return View(log);
    }

	[HttpPost]
	[ValidateAntiForgeryToken]
    public async Task<IActionResult> GetServiceLog(DtRequest request)
    {
        try
		{
			var query = _context.ServiceLog;
			var jsonData = await query.GetDatatableResponseAsync<Log, ServiceLogIndexViewModel>(request);

            return Ok(jsonData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating ServiceLog index json");
        }
        
        return StatusCode(500);
    }

    private string PrettyXml(string xml)
    {
        var stringBuilder = new StringBuilder();

        var element = XElement.Parse(xml);

        var settings = new XmlWriterSettings();
        settings.OmitXmlDeclaration = true;
        settings.Indent = true;
        settings.NewLineOnAttributes = true;

        using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
        {
            element.Save(xmlWriter);
        }

        return stringBuilder.ToString();
    }
}

