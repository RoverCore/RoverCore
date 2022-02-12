using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using RoverCore.Boilerplate.Domain.Entities.Templates;

namespace RoverCore.Boilerplate.Infrastructure.Services.Templates
{
    public class VirtualFileInfo : IFileInfo
    {
	    private readonly Template? _template;

	    public VirtualFileInfo(Template? template)
        {
	        _template = template;
	        Exists = template is not null;
	        LastModified = DateTime.SpecifyKind(_template?.Updated ?? DateTime.UtcNow, DateTimeKind.Utc);
            Length = Exists ? template!.Body.Length : -1;
        }

        public bool Exists { get; }

        public bool IsDirectory => false;

        public DateTimeOffset LastModified { get; }

        public long Length { get; }

        public string Name => _template?.Slug ?? string.Empty;

#pragma warning disable CS8603 // Possible null reference return.
        public string PhysicalPath => null;
#pragma warning restore CS8603 // Possible null reference return.

        public Stream CreateReadStream()
        {
	        return new MemoryStream(Encoding.UTF8.GetBytes(_template?.Body ?? ""));
        }
    }
}
