using ELearning.Application.Interfaces;
using ELearning.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ELearning.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")]
public class ProgressController : ControllerBase
{
    private readonly ILessonProgressService _progressService;

    public ProgressController(ILessonProgressService progressService)
        => _progressService = progressService;

    // POST /api/progress/{lessonId}
    [HttpPost("{lessonId:guid}")]
    public async Task<IActionResult> MarkComplete(Guid lessonId)
    {
        var studentId = GetUserId();
        var result = await _progressService.MarkLessonCompleteAsync(studentId, lessonId);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    // GET /api/progress/{courseId}
    [HttpGet("{courseId:guid}")]
    public async Task<IActionResult> GetCourseProgress(Guid courseId)
    {
        var studentId = GetUserId();
        var result = await _progressService.GetCourseProgressAsync(studentId, courseId);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    private Guid GetUserId()
        => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}