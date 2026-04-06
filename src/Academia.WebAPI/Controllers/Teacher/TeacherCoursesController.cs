using Academia.Application.Courses.Queries.GetCourseById;
using Academia.Application.Courses.Queries.GetCourses;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers.Teacher;

[ApiController]
[Route("api/teacher/courses")]
[Authorize(Policy = "AdminOrTeacher")]
public class TeacherCoursesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;

    public TeacherCoursesController(IMediator mediator, ICurrentUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyCourses(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetCoursesQuery(page, pageSize, TeacherId: _currentUser.Id), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCourseByIdQuery(id), ct);
        return Ok(result);
    }
}
