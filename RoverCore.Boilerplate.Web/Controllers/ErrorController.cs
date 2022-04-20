using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace RoverCore.Boilerplate.Web.Controllers;

[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : Controller
{
    [Route("error/{code}")]
    public IActionResult Index(int? code = null)
    {
        int[] available = { 401, 404, 500 };

        if (code.HasValue)
        {
            if (available.Contains(code.Value))
            {
                var viewName = code.ToString();
                return View(viewName);
            }
        }
        return View();
    }

}