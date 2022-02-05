using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RoverCore.Boilerplate.Infrastructure.Extensions;

public static class ModelStateExtensions
{
    /// <summary>
    /// Remove any modelstate errors that don't pertain to the actual fields we are binding
    /// </summary>
    /// <param name="modelState"></param>
    /// <param name="bindingFields"></param>
    public static void Scrub(this ModelStateDictionary modelState, string bindingFields)
    {
        string[] bindingKeys = bindingFields.Split(",");
        foreach (string key in modelState.Keys)
        {
            if (!bindingKeys.Contains(key))
                modelState.Remove(key);
        }
    }
}