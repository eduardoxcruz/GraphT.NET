using GraphT.Controllers.FindFinishedTasks;
using GraphT.Controllers.FindInProgressTasks;
using GraphT.Controllers.FindReadyToStartTasks;

using Microsoft.AspNetCore.Mvc;

namespace GraphT.WebAPI.Controllers.TaskStatuses;

[ApiController]
[Route("api/tasks/statuses")]
public class TaskStatusesController : ControllerBase
{
    private readonly IFindFinishedTasksController _findFinishedTasksController;
    private readonly IFindInProgressTasksController _findInProgressTasksController;
    private readonly IFindReadyToStartTasksController _findReadyToStartTasksController;

    public TaskStatusesController(
        IFindFinishedTasksController findFinishedTasksController,
        IFindInProgressTasksController findInProgressTasksController,
        IFindReadyToStartTasksController findReadyToStartTasksController)
    {
        _findFinishedTasksController = findFinishedTasksController;
        _findInProgressTasksController = findInProgressTasksController;
        _findReadyToStartTasksController = findReadyToStartTasksController;
    }

    [HttpGet("finished")]
    public async Task<ActionResult<UseCases.FindFinishedTasks.OutputDto>> GetFinishedTasks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _findFinishedTasksController.RunUseCase(new UseCases.FindFinishedTasks.InputDto
        {
            PagingParams = new() { PageNumber = pageNumber, PageSize = pageSize }
        });
        return Ok(result);
    }

    [HttpGet("in-progress")]
    public async Task<ActionResult<UseCases.FindInProgressTasks.OutputDto>> GetInProgressTasks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _findInProgressTasksController.RunUseCase(new UseCases.FindInProgressTasks.InputDto
        {
            PagingParams = new() { PageNumber = pageNumber, PageSize = pageSize }
        });
        return Ok(result);
    }

    [HttpGet("ready-to-start")]
    public async Task<ActionResult<UseCases.FindReadyToStartTasks.OutputDto>> GetReadyToStartTasks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _findReadyToStartTasksController.RunUseCase(new UseCases.FindReadyToStartTasks.InputDto
        {
            PagingParams = new() { PageNumber = pageNumber, PageSize = pageSize }
        });
        return Ok(result);
    }
}
