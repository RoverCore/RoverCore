using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoverCore.Boilerplate.Domain.DTOs.Authentication;
using RoverCore.Boilerplate.Domain.Entities.Identity;

namespace RoverCore.Boilerplate.Infrastructure.Authentication.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model);
        Task<ApplicationUser?> GetById(string id);
    }
}
