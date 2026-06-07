using System.Security.Claims;
using ELearning.Application.DTOs.Courses;
using ELearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearning.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    // GET /api/courses
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _courseService.GetPublishedCoursesAsync();
        return Ok(result.Value);
    }

    // GET /api/courses/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _courseService.GetCourseDetailsAsync(id);
        if (!result.IsSuccess)
            return NotFound(new { message = result.Error });
        return Ok(result.Value);
    }

    // GET /api/courses/my
    [HttpGet("my")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMyCourses()
    {
        var result = await _courseService.GetTeacherCoursesAsync(GetUserId());
        return Ok(result.Value);
    }

    // POST /api/courses
    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create(CreateCourseRequest request)
    {
        var result = await _courseService.CreateCourseAsync(GetUserId(), request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    // PUT /api/courses/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Update(Guid id, UpdateCourseRequest request)
    {
        var result = await _courseService.UpdateCourseAsync(id, GetUserId(), request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    // POST /api/courses/{id}/publish
    [HttpPost("{id}/publish")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var result = await _courseService.PublishCourseAsync(id, GetUserId());
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(new { message = "تم نشر الكورس بنجاح" });
    }

    // DELETE /api/courses/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _courseService.DeleteCourseAsync(id, GetUserId());
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(new { message = "تم حذف الكورس بنجاح" });
    }

    // POST /api/courses/{id}/sections
    [HttpPost("{id}/sections")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> AddSection(Guid id, CreateSectionRequest request)
    {
        var result = await _courseService.AddSectionAsync(id, GetUserId(), request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    // DELETE /api/courses/sections/{sectionId}
    [HttpDelete("sections/{sectionId}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteSection(Guid sectionId)
    {
        var result = await _courseService.DeleteSectionAsync(sectionId, GetUserId());
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(new { message = "تم حذف الـ Section بنجاح" });
    }

    // POST /api/courses/sections/{sectionId}/lessons
    [HttpPost("sections/{sectionId}/lessons")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> AddLesson(Guid sectionId, CreateLessonRequest request)
    {
        var result = await _courseService.AddLessonAsync(sectionId, GetUserId(), request);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    // DELETE /api/courses/lessons/{lessonId}
    [HttpDelete("lessons/{lessonId}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteLesson(Guid lessonId)
    {
        var result = await _courseService.DeleteLessonAsync(lessonId, GetUserId());
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(new { message = "تم حذف الـ Lesson بنجاح" });
    }
}