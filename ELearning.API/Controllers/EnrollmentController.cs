using ELearning.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ELearning.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
        => _enrollmentService = enrollmentService;

    // POST /api/enrollments/{courseId}
    [HttpPost("{courseId:guid}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Enroll(Guid courseId)
    {
        var studentId = GetUserId();
        var result = await _enrollmentService.EnrollAsync(studentId, courseId);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    // GET /api/enrollments/my
    [HttpGet("my")]
    public async Task<IActionResult> GetMyEnrollments()
    {
        var studentId = GetUserId();
        var result = await _enrollmentService.GetMyEnrollmentsAsync(studentId);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    // DELETE /api/enrollments/{courseId}
    [HttpDelete("{courseId:guid}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Unenroll(Guid courseId)
    {
        var studentId = GetUserId();
        var result = await _enrollmentService.UnenrollAsync(studentId, courseId);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Error);
    }

    private Guid GetUserId()
        => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}