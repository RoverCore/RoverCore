using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperion.Web.Controllers;
using HyperionCore.Web.Areas.Identity.Controllers;

namespace Microsoft.AspNetCore.Mvc;

public static class UrlHelperExtensions
{
    public static string EmailConfirmationLink(this IUrlHelper urlHelper, int userId, string code, string scheme)
    {
        return urlHelper.Action(
            action: nameof(AccountController.ConfirmEmail),
            controller: "Account",
            values: new { userId, code },
            protocol: scheme);
    }

    public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, int userId, string code, string scheme)
    {
        return urlHelper.Action(
            action: nameof(AccountController.ResetPassword),
            controller: "Account",
            values: new { userId, code },
            protocol: scheme);
    }
}