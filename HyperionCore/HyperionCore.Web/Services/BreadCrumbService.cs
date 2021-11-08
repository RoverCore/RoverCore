﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UrlHelper = Microsoft.AspNetCore.Mvc.Routing.UrlHelper;

namespace Hyperion.Web.Services
{
    public class BreadCrumbService : IBreadCrumbService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public List<BreadCrumb> BreadCrumbs { get; set; }

        public BreadCrumbService(IHttpContextAccessor httpContextAccessor)
        {
            BreadCrumbs = new List<BreadCrumb>();
            _httpContextAccessor = httpContextAccessor;

            Add("Home", "/admin/dashboard");
        }

        public void Add (string title, string url = null)
        {
            BreadCrumbs.Add(new BreadCrumb
            {
                Title = title,
                Url = url
            });
        }
    }
}
