using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoverCore.Boilerplate.Domain.Entities;
using RoverCore.Boilerplate.Infrastructure.Extensions;
using RoverCore.Boilerplate.Infrastructure.Models.AuthenticationModels;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;
using RoverCore.Boilerplate.Infrastructure.Services;
using RoverCore.Boilerplate.Web.Areas.Api.Models;
using RoverCore.Boilerplate.Web.Helpers;

namespace RoverCore.Boilerplate.Web.Areas.Api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
[Area("Api")]
public class MembersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;

    public MembersController(ApplicationDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    /// <summary>
    /// Authenticate a member
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<ApiResponse> Authenticate(AuthenticateRequest model)
    {
        var member = await _userService.Authenticate(model);

        if (member == null)
            return new ApiResponse(System.Net.HttpStatusCode.NotFound, model, "Email or password is incorrect");

        return new ApiResponse(System.Net.HttpStatusCode.OK, member);
    }

    // GET: api/v1/Members/{id}
    /// <summary>
    /// Returns a specific member
    /// </summary>
    /// <param name="id"></param>    
    [HttpGet("{id}")]
    public async Task<ApiResponse> GetMember(int id)
    {
        // TODO: Add validation for an id
        var httpUser = (Member)HttpContext.Items["User"];

        var member = await _context.Member.FirstOrDefaultAsync(m => m.MemberId == id);

        // TODO: Strip all data that isn't supposed to be public from this api response

        return new ApiResponse(System.Net.HttpStatusCode.OK, member);
    }

    private const string CreateMemberBindingFields = "FirstName,LastName,Email,Password";
    private const string UpdateMemberBindingFields = "FirstName,LastName,Email";

    // POST: api/v1/Members/
    /// <summary>
    /// Create a new member
    /// </summary>
    /// <param name="member"></param>
    [AllowAnonymous]
    [HttpPost]
    public async Task<ApiResponse> CreateMember([Bind(CreateMemberBindingFields)] Member member)
    {
        var safemember = new Member
        {
            Email = member.Email?.Trim() ?? "",
            FirstName = member.FirstName?.Trim() ?? "",
            LastName = member.LastName?.Trim() ?? "",
            Password = member.Password ?? ""
        };

        TryValidateModel(safemember);
        ModelState.Scrub(CreateMemberBindingFields);  // Remove all errors that aren't related to the binding fields

        if (!ModelState.IsValid)
        {
            // Return all validation errors
            return new ApiResponse(System.Net.HttpStatusCode.BadRequest, null, "An error has occurred", ModelState);
        }

        // Check to see if member already exists
        var _membercheck = await _context.Member.FirstOrDefaultAsync(m => m.Email == safemember.Email);
        if (_membercheck != null)
            return new ApiResponse(System.Net.HttpStatusCode.BadRequest, null, "An account for this email already exists");

        // Securely hash the member password
        PasswordHash ph = PasswordHasher.Hash(member.Password ?? "");
        safemember.Password = ph.HashedPassword;
        safemember.PasswordSalt = ph.Salt;

        _context.Member.Add(safemember);
        await _context.SaveChangesAsync();

        return new ApiResponse(System.Net.HttpStatusCode.OK, safemember);
    }

    // PUT: api/v1/members/
    /// <summary>
    /// Update an existing member
    /// </summary>
    /// <param name="member"></param>   
    [HttpPut]
    public async Task<ApiResponse> UpdateMember([Bind(UpdateMemberBindingFields)] Member member)
    {
        var httpUser = (Member)HttpContext.Items["User"];
        var newMember = await _context.Member.FirstOrDefaultAsync(m => m.MemberId == httpUser.MemberId);

        if (newMember == null)
        {
            return new ApiResponse(System.Net.HttpStatusCode.NotFound, null, "User not found");
        }

        newMember.FirstName = member.FirstName ?? newMember.FirstName;
        newMember.LastName = member.LastName ?? newMember.LastName;

        TryValidateModel(newMember);
        ModelState.Scrub(UpdateMemberBindingFields);  // Remove all errors that aren't related to the binding fields

        // Add custom errors to fields
        //ModelState.AddModelError("Email", "Something else with email is wrong");

        if (!ModelState.IsValid)
        {
            // Return all validation errors
            return new ApiResponse(System.Net.HttpStatusCode.BadRequest, null, "An error has occurred", ModelState);
        }

        _context.Member.Update(newMember);
        await _context.SaveChangesAsync();

        return new ApiResponse(System.Net.HttpStatusCode.OK, newMember);

    }

    // DELETE: api/v1/Members/{id}
    /// <summary>
    /// Delete a member
    /// </summary>
    /// <param name="id"></param>   
    [HttpDelete("{id}")]
    public async Task<bool> DeleteMember(int id)
    {
        await Task.Delay(0);

        return true;
    }


}