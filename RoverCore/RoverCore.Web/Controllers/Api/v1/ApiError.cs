using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rover.Web.Controllers.Api.v1;

public class ApiError
{
    public string Key { get; set; }
    public List<string> Errors { get; set; }
}