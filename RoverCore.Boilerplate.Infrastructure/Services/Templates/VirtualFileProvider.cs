using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;

namespace RoverCore.Boilerplate.Infrastructure.Services.Templates;

/// <summary>
/// This class allows FluentEmail to include templates that are stored in the database by using their slug names as a file path.
/// </summary>
public class VirtualFileProvider : IFileProvider
{
    public List<Domain.Entities.Templates.Template> Templates { get; set; }

    public VirtualFileProvider()
    {
	    Templates = new List<Domain.Entities.Templates.Template>();
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        throw new NotImplementedException();
    }

    public IFileInfo GetFileInfo(string subpath)
    {
	    var template = Templates.Find(t => t.Slug == subpath);

	    return new VirtualFileInfo(template);
    }

    public IChangeToken Watch(string filter)
    {
        throw new NotImplementedException();
    }
}

