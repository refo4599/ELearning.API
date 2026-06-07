using System.Security.Claims;
using ELearning.Application.DTOs.Profile;
using ELearning.Application.Interfaces;
using ELearning.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearning.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private string GetRole() =>
        User.FindFirst(ClaimTypes.Role)?.Value ?? "";

    // POST /api/profile/complete/student
    [HttpPost("complete/student")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CompleteStudentProfile(
        CompleteStudentProfileRequest request)
    {
        var result = await _profileService
            .CompleteStudentProfileAsync(GetUserId(), request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result.Value);
    }

    // POST /api/profile/complete/teacher
    [HttpPost("complete/teacher")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CompleteTeacherProfile(
        CompleteTeacherProfileRequest request)
    {
        var result = await _profileService
            .CompleteTeacherProfileAsync(GetUserId(), request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result.Value);
    }

    // GET /api/profile/me
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var role = GetRole();
        var userId = GetUserId();

        if (role == UserRole.Student.ToString())
        {
            var result = await _profileService.GetStudentProfileAsync(userId);
            return Ok(result.Value);
        }
        else if (role == UserRole.Teacher.ToString())
        {
            var result = await _profileService.GetTeacherProfileAsync(userId);
            return Ok(result.Value);
        }

        return BadRequest(new { message = "Role غير معروف" });
    }
}