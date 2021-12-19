using Azersun.Audit.Utilities;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Enumerations;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
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
    /// Add new firm
    /// </summary>
    /// <param name="organization"></param>
    /// <returns>New Organization id</returns>
    /// <response code="200">New Organization is added</response>
    /// <response code="400">Params is not correct</response>  
    /// <response code="401">Unauthorized</response> 
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

}
