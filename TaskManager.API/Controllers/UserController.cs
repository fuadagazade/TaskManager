using Azersun.Audit.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;
using TaskManager.Core.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        /// <summary>
        /// Get Users
        /// </summary>
        /// <returns>Users list</returns>
        /// <response code="200">Returns users list</response>
        /// <response code="204">Users list is empty</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Don't have permissions</response>
        [HttpGet]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Get()
        {
            IEnumerable<User> data = await this._userService.Get();

            if (data is null || data.Count<User>() == 0)
            {
                return StatusCode(StatusCodes.Status204NoContent, new List<User>());
            }

            return Ok(data);
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Organization</returns>
        /// <response code="200">Returns user</response>
        /// <response code="204">User not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            User data = await this._userService.Get(long.Parse(id));

            if (data is null)
            {
                return StatusCode(StatusCodes.Status204NoContent, new User());
            }

            return Ok(data);
        }

        /// <summary>
        /// Get Users Table
        /// </summary>
        /// <returns>Users Table</returns>
        /// <response code="200">Returns Users Table</response>
        /// <response code="204">Users list is empty</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Don't have permissions</response>
        [HttpGet("table")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> Get(Table table)
        {
            IEnumerable<Claim> claims = (User.Identity as ClaimsIdentity).Claims;

            string organizationId = claims.Where((c) => c.Type == "Organization").FirstOrDefault().Value;

            TableResponse<User> data = await this._userService.GetByOrganization(long.Parse(organizationId), table);

            if (data is null)
            {
                return StatusCode(StatusCodes.Status204NoContent, new TableResponse<Organization>());
            }

            return Ok(data);
        }

        /// <summary>
        /// Add new User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>New Organization id</returns>
        /// <response code="200">New Organization is added</response>
        /// <response code="400">Params is not correct</response>  
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] User user)
        {

            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ModelState.ValidationState);
            }

            IEnumerable<Claim> claims = (User.Identity as ClaimsIdentity).Claims;

            string organizationId = claims.Where((c) => c.Type == "Organization").FirstOrDefault().Value;

            if (organizationId != user.OrganizationId.ToString())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new User());
            }

            user.Password = Cryptographer.Encrypt(user.Password);

            long userId = await this._userService.Add(user);

            if (userId > 0)
            {
                return Ok(userId);
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        /// <summary>
        /// Update Organization
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Result of operation</returns>
        /// <response code="200">Organization Updated</response>
        /// <response code="400">Params is not correct</response>  
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Don't have permissions</response>
        [HttpPut]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Put([FromBody] User user)
        {
            IEnumerable<Claim> claims = (User.Identity as ClaimsIdentity).Claims;

            string userID = claims.Where((c) => c.Type == "Id").FirstOrDefault().Value;
            string organizationId = claims.Where((c) => c.Type == "Organization").FirstOrDefault().Value;
            string role = claims.Where((c) => c.Type == ClaimTypes.Role).FirstOrDefault().Value;

            if(role == "User" && userID != user.Id.ToString())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new User());
            }

            if (organizationId != user.OrganizationId.ToString())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new User());
            }

            bool result = await this._userService.Update(user);

            return result ? Ok() : StatusCode(StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="passwords"></param>
        /// <returns>Result of operation</returns>
        /// <response code="200">Password Updated</response>
        /// <response code="400">Params is not correct</response>  
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Don't have permissions</response>
        [HttpPut("password")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Put([FromBody] PasswordUpdate passwords)
        {
            IEnumerable<Claim> claims = (User.Identity as ClaimsIdentity).Claims;

            string userId = claims.Where((c) => c.Type == "Id").FirstOrDefault().Value;

            bool result = await this._userService.UpdatePassword(long.Parse(userId), passwords.CurrentEncrypted, passwords.ChangedEncrypted);

            return result ? Ok() : StatusCode(StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Result of operation</returns>
        /// <response code="200">User Deleted</response>
        /// <response code="400">Params is not correct</response>  
        /// <response code="401">Unauthorized</response> 
        /// <response code="403">Don't have permissions</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            bool result = await this._userService.Delete(id);

            if (result)
                return Ok(result);
            else
                return StatusCode(StatusCodes.Status400BadRequest);
        }
    }
}
