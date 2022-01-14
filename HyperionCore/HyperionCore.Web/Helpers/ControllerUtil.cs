using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hyperion.Web.Helpers;

public static class ControllerUtil
{
    // Remove any modelstate errors that don't pertain to the actual fields we are binding to
    public static void Scrub(this ModelStateDictionary ModelState, string bindingFields)
    {
        string[] bindingKeys = bindingFields.Split(",");
        foreach (string key in ModelState.Keys)
        {
            if (!bindingKeys.Contains(key))
                ModelState.Remove(key);
        }
    }
}