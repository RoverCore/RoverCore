using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Audit.EntityFramework;
using Microsoft.EntityFrameworkCore;
using RoverCore.Abstractions.Templates;

namespace RoverCore.Boilerplate.Domain.Entities.Templates;

[AuditInclude]
[Index(nameof(Slug), IsUnique = true)]
public class Template : ITemplate
{
    [Key]
    public int Id { get; set; }
    [Required]
    [RegularExpression(@"^[a-z0-9\-_]{1,40}$", ErrorMessage = "The slug may contain only lowercase letters and numbers.")]
    public string Slug { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    [DisplayName("Header Summary Text")]
    public string PreHeader { get; set; }
    public string Body { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}

