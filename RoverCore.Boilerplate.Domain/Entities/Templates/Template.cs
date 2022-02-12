using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RoverCore.Boilerplate.Domain.Entities.Templates;

[Index(nameof(Slug), IsUnique = true)]
public class Template
{
    [Key]
    public int Id { get; set; }
    [Required]
    [RegularExpression(@"^[a-z0-9\-_]{1,40}$", ErrorMessage = "The slug may contain only lowercase letters and numbers.")]
    public string Slug { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Body { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}

