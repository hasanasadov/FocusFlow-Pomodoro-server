using Application.Features.UserTasks.Commands.CompleteTask;
using Application.Features.UserTasks.Commands.CreateUserTask;
using Application.Features.UserTasks.Commands.DeleteUserTask;
using Application.Features.UserTasks.Commands.UpdateUserTask;
using Application.Features.UserTasks.Queries.GetAllLabelsOfUser;
using Application.Features.UserTasks.Queries.GetAllUserTask;
using Application.Features.UserTasks.Queries.GetAllUserTasksByLabel;
using Application.Features.UserTasks.Queries.GetByIdUserTask;
using Application.Features.UserTasks.Queries.GetGroupByPriorityUserTask;


namespace WebAPI.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserTaskController(IUserTaskService userTaskService) : BaseController
{
    private readonly IUserTaskService userTaskService = userTaskService;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType(typeof(IEnumerable<UserTaskDto>))]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        Result<IReadOnlyCollection<UserTaskDto>> result = await Mediator.Send(new GetAllUserTaskQuery(), cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType(typeof(UserTaskDto))]
    public async Task<IActionResult> GetByid(int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetByIdUserTaskQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpGet("priority")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType(typeof(IEnumerable<UserTaskDto>))]
    public async Task<IActionResult> GetByPriority(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetGroupByPriorityUserTaskQuery(), cancellationToken);
        if (result.IsFailure) 
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpGet("labels")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType(typeof(IReadOnlyCollection<string>))]  
    public async Task<IActionResult> GetAllLabels(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllLabelsOfUserQuery(), cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("labels/{label}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType(typeof(IReadOnlyCollection<string>))]
    public async Task<IActionResult> GetAllUserTaskByLabel(string label, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllUserTasksByLabelQuery(label), cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("activity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType(typeof(IReadOnlyCollection<bool>))]
    public async Task<IActionResult> Get28DaysActivity(CancellationToken cancellationToken)
    {
        var result = await userTaskService.Get28DayActivity(cancellationToken);
        if (result.IsFailure)
        {
            return FromResult(result);
        }
        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateUserTaskCommand request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return FromResult(result);
        }
        return Created();
    }

    [HttpPost("list")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] List<CreateUserTaskListDto> request, CancellationToken cancellationToken)
    {
        var result = await userTaskService.CreateListTasks(request, cancellationToken);
        if (result.IsFailure)
        {
            return FromResult(result);
        }
        return Created();
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromBody] UpdateUserTaskCommand request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return FromResult(result);
        }
        return NoContent();
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Patch([FromBody] UpdateUserTaskPriorityCommand request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return FromResult(result);
        }
        return NoContent();
    }

    [HttpPatch("complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteTask(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return FromResult(result);
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteUserTaskCommand(id), cancellationToken);
        if (result.IsFailure)
        {
            return FromResult(result);
        }
        return NoContent();
    }
}
