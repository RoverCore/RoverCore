using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RoverCore.Web.Helpers;

public static class ControllerUtil
{
    /// <summary>
    /// Remove any modelstate errors that don't pertain to the actual fields we are binding
    /// </summary>
    /// <param name="ModelState"></param>
    /// <param name="bindingFields"></param>
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