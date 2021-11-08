using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hyperion.Web.Services
{
    public interface IBreadCrumbService
    {
        List<BreadCrumb> BreadCrumbs { get; set; }

        void Add(string title, string url = null);

    }
}
