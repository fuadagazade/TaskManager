using Azersun.Audit.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Core.Enumerations;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;
using TaskManager.Core.ViewModels;

namespace TaskManager.API.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _organizationService;
    private readonly IUserService _userService;


    public OrganizationController(IOrganizationService organizationService, IUserService userService)
    {
        this._organizationService = organizationService;
        this._userService = userService;
    }

    /// <summary>
    /// Add new Organization
    /// </summary>
    /// <param name="data"></param>
    /// <returns>New Organization id</returns>
    /// <response code="200">New Organization is added</response>
    /// <response code="400">Params is not correct</response>  
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] OrganizationRegistration data)
    {

        if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ModelState.ValidationState);
        }

        Organization organization = new Organization
        {
            Name = data.Name,
            Tag = data.Tag.ToLower(),
            Phone = data.Phone,
            Address = data.Address,
            Status = Status.Active // be Passive if realize Approve System
        };


        long organizationId = await this._organizationService.Add(organization);

        User user = new User
        {
            FirstName = data.FirstName,
            LastName = data.LastName,
            Email = data.Email.ToLower(),
            Password = Cryptographer.Encrypt(data.Password),
            Role = Role.Admin,
            Approved = true, // be False if realize Approve System
            OrganizationId = organizationId,
            Status = Status.Active
        };

        long userId = await this._userService.Add(user);

        if (organizationId > 0 && userId > 0)
        {
            return Ok(organizationId);
        }
        else
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Get Organizations
    /// </summary>
    /// <returns>Organizations list</returns>
    /// <response code="200">Returns organizations list</response>
    /// <response code="204">Organizations list is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Don't have permissions</response>
    [HttpGet]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> Get()
    {
        IEnumerable<Organization> data = await this._organizationService.Get();

        if (data is null || data.Count<Organization>() == 0)
        {
            return StatusCode(StatusCodes.Status204NoContent, new List<Organization>());
        }

        return Ok(data);
    }

    /// <summary>
    /// Get Organization
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Organization</returns>
    /// <response code="200">Returns organization</response>
    /// <response code="204">Organization not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Don't have permissions</response>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(string id)
    {
        IEnumerable<Claim> claims = (User.Identity as ClaimsIdentity).Claims;

        string organizationId = claims.Where((c) => c.Type == "Organization").FirstOrDefault().Value;

        if (organizationId != id)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new Organization());
        }

        Organization data = await this._organizationService.Get(long.Parse(id));

        if (data is null)
        {
            return StatusCode(StatusCodes.Status204NoContent, new Organization());
        }

        return Ok(data);
    }

    /// <summary>
    /// Get Organizations Table
    /// </summary>
    /// <returns>Organizations Table</returns>
    /// <response code="200">Returns Organizations Table</response>
    /// <response code="204">Organizations list is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Don't have permissions</response>
    [HttpGet("table")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> Get(Table table)
    {
        TableResponse<Organization> data = await this._organizationService.Get(table);

        if (data is null)
        {
            return StatusCode(StatusCodes.Status204NoContent, new TableResponse<Organization>());
        }

        return Ok(data);
    }

    /// <summary>
    /// Update Organization
    /// </summary>
    /// <param name="organization"></param>
    /// <returns>Result of operation</returns>
    /// <response code="200">Organization Updated</response>
    /// <response code="400">Params is not correct</response>  
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Don't have permissions</response>
    [HttpPut]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<IActionResult> Put([FromBody] OrganizationUpdate organization)
    {
        IEnumerable<Claim> claims = (User.Identity as ClaimsIdentity).Claims;

        string organizationId = claims.Where((c) => c.Type == "Organization").FirstOrDefault().Value;

        if (organizationId != organization.Id.ToString())
        {
            return StatusCode(StatusCodes.Status403Forbidden, new Organization());
        }

        bool result = await this._organizationService.Update(organization);

        return result ? Ok() : StatusCode(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Delete Organization
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Result of operation</returns>
    /// <response code="200">Organization Deleted</response>
    /// <response code="400">Params is not correct</response>  
    /// <response code="401">Unauthorized</response> 
    /// <response code="403">Don't have permissions</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> Delete(int id)
    {
        bool result = await this._organizationService.Delete(id);

        if (result)
            return Ok(result);
        else
            return StatusCode(StatusCodes.Status400BadRequest);
    }

}
