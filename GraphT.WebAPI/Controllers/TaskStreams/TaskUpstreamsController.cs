using GraphT.Controllers.FindTaskUpstreamsById;
using GraphT.UseCases.FindTaskUpstreamsById;

using Microsoft.AspNetCore.Mvc;

namespace GraphT.WebAPI.Controllers.TaskStreams;

[ApiController]
[Route("api/tasks/streams/upstreams")]
public class TaskUpstreamsController : ControllerBase
{
    private readonly IFindTaskUpstreamsByIdController _controller;

    public TaskUpstreamsController(IFindTaskUpstreamsByIdController controller)
    {
        _controller = controller;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OutputDto>> GetUpstreams(Guid id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        OutputDto result = await _controller.RunUseCase(new InputDto
        {
            Id = id,
            PagingParams = new() { PageNumber = pageNumber, PageSize = pageSize }
        });
        return Ok(result);
    }
}