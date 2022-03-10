using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentEmail.Core.Models;
using RoverCore.Abstractions.Seeder;
using RoverCore.Boilerplate.Domain.Entities.Templates;
using RoverCore.Boilerplate.Infrastructure.Common.Templates.Models;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;

namespace RoverCore.Boilerplate.Infrastructure.Common.Templates.Seeding
{
    public class TemplateSeed : ISeeder
    {
        private readonly ApplicationDbContext _context; 
        
        public int Priority { get; } = 0;

        public TemplateSeed(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task SeedAsync()
        {
            if (!_context.Template.Any())
            {
                _context.Template.Add(new Template
                {
                    Name = "Default Layout", 
                    Slug = TemplateSlugs.Layout, 
                    Body = LoadTemplate("_layout.liquid"), 
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow, 
                    Description = "Default layout template"
                });

                _context.Template.Add(new Template
                {
                    Name = "Change Password",
                    Slug = TemplateSlugs.ChangePassword,
                    Body = LoadTemplate("changepassword.liquid"),
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    Description = "The email will be sent each time a user requests a password change. "
                });

                _context.Template.Add(new Template
                {
                    Name = "Locked Account",
                    Slug = TemplateSlugs.LockedAccount,
                    Body = LoadTemplate("lockedaccount.liquid"),
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    Description = "A user will receive this email when their account is blocked due to suspicious login attempts."
                });

                _context.Template.Add(new Template
                {
                    Name = "Verification Email",
                    Slug = TemplateSlugs.VerificationEmail,
                    PreHeader = "Confirm your email address for the site.",
                    Body = LoadTemplate("verificationemail.liquid"),
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    Description = "A new user will receive this email when they sign up or log in for the first time."
                });

                _context.Template.Add(new Template
                {
                    Name = "Welcome",
                    Slug = TemplateSlugs.Welcome,
                    Body = LoadTemplate("welcome.liquid"),
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    Description = "A new user will receive this email when they sign up or log in for the first time."
                });

                await _context.SaveChangesAsync();
            }
        }

        private string LoadTemplate(string resourceName)
        {
            var res = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .SingleOrDefault(str => str.EndsWith(resourceName));

            if (res != null)
            {
                string body;
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(res);

                if (stream != null)
                {
                    TextReader tr = new StreamReader(stream);
                    body = tr.ReadToEnd();
                    return body;
                }
            }

            return string.Empty;
        }
    }
}
