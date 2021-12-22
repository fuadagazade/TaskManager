using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;
using TaskManager.Core.ViewModels;
using Task = TaskManager.Core.Models.Task;


namespace TaskManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        this._taskService = taskService;
    }

    /// <summary>
    /// Add new Task
    /// </summary>
    /// <param name="data"></param>
    /// <returns>New Organization id</returns>
    /// <response code="200">New Task is added</response>
    /// <response code="400">Params is not correct</response>  
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] Task data)
    {

        if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ModelState.ValidationState);
        }

        IEnumerable<Claim> claims = (User.Identity as ClaimsIdentity).Claims;

        string organizationId = claims.Where((c) => c.Type == "Organization").FirstOrDefault().Value;
        string userId = claims.Where((c) => c.Type == "Id").FirstOrDefault().Value;


        if (organizationId != data.OrganizationId.ToString())
        {
            return StatusCode(StatusCodes.Status403Forbidden, new Task());
        }

       data.CreatorId = long.Parse(userId);

        long result = await this._taskService.Add(data);

        if (result > 0)
        {
            return Ok(result);
        }
        else
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Update Task
    /// </summary>
    /// <param name="task"></param>
    /// <returns>Result of operation</returns>
    /// <response code="200">Organization Updated</response>
    /// <response code="400">Params is not correct</response>  
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Don't have permissions</response>
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Put([FromBody] Task task)
    {
        IEnumerable<Claim> claims = (User.Identity as ClaimsIdentity).Claims;

        string organizationId = claims.Where((c) => c.Type == "Organization").FirstOrDefault().Value;

        if (organizationId != task.OrganizationId.ToString())
        {
            return StatusCode(StatusCodes.Status403Forbidden, new Task());
        }

        bool result = await this._taskService.Update(task);

        return result ? Ok() : StatusCode(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Assign Task
    /// </summary>
    /// <param name="assign"></param>
    /// <returns>Result of operation</returns>
    /// <response code="200">Assigned</response>
    /// <response code="400">Params is not correct</response>  
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Don't have permissions</response>
    [HttpPut("assign")]
    [Authorize]
    public async Task<IActionResult> Assign([FromBody] Assigment assign)
    {
        long result = await this._taskService.Assign(assign);

        if (result == 0) StatusCode(StatusCodes.Status400BadRequest);

        return Ok(result);
    }

    /// <summary>
    /// UnAssign Task
    /// </summary>
    /// <param name="assign"></param>
    /// <returns>Result of operation</returns>
    /// <response code="200">Unassigned</response>
    /// <response code="400">Params is not correct</response>  
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Don't have permissions</response>
    [HttpPut("unassign")]
    [Authorize]
    public async Task<IActionResult> unassign([FromBody] Assigment assign)
    {
        bool result = await this._taskService.DeleteAssigment(assign);

        return result ? Ok() : StatusCode(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Delete Task
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Result of operation</returns>
    /// <response code="200">Task Deleted</response>
    /// <response code="400">Params is not correct</response>  
    /// <response code="401">Unauthorized</response> 
    /// <response code="403">Don't have permissions</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        bool result = await this._taskService.Delete(id);

        if (result)
            return Ok(result);
        else
            return StatusCode(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Get Task
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Task</returns>
    /// <response code="200">Returns task</response>
    /// <response code="204">Task not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Don't have permissions</response>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(string id)
    {
        TaskView data = await this._taskService.Get(long.Parse(id));

        if (data is null)
        {
            return StatusCode(StatusCodes.Status204NoContent, new TaskView());
        }

        return Ok(data);
    }

    /// <summary>
    /// Get tasks Table
    /// </summary>
    /// <returns>Tasks Table</returns>
    /// <response code="200">Returns Tasks Table</response>
    /// <response code="204">Tasks list is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Don't have permissions</response>
    [HttpGet("table/{id}")]
    [Authorize]
    public async Task<IActionResult> Get(string id,Table table)
    {
        IEnumerable<Claim> claims = (User.Identity as ClaimsIdentity).Claims;

        string organizationId = claims.Where((c) => c.Type == "Organization").FirstOrDefault().Value;

        if (organizationId != id)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new Task());
        }
        TableResponse<TaskView> data = await this._taskService.Get(long.Parse(id),table);

        if (data is null)
        {
            return StatusCode(StatusCodes.Status204NoContent, new TableResponse<TaskView>());
        }

        return Ok(data);
    }

    /// <summary>
    /// Get tasks Table
    /// </summary>
    /// <returns>Tasks Table</returns>
    /// <response code="200">Returns Tasks Table</response>
    /// <response code="204">Tasks list is empty</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Don't have permissions</response>
    [HttpGet("my/{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserTasks(string id, Table table)
    {
        IEnumerable<Claim> claims = (User.Identity as ClaimsIdentity).Claims;

        string userId = claims.Where((c) => c.Type == "Id").FirstOrDefault().Value;

        if (userId != id)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new TableResponse<TaskView>());
        }
        TableResponse<TaskView> data = await this._taskService.UserTasks(long.Parse(id), table);

        if (data is null)
        {
            return StatusCode(StatusCodes.Status204NoContent, new TableResponse<TaskView>());
        }

        return Ok(data);
    }
}

