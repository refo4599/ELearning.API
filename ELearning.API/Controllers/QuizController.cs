using ELearning.Application.DTOs.Quizzes;
using ELearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ELearning.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
        => _quizService = quizService;

    // GET /api/quiz/lesson/{lessonId}
    [HttpGet("lesson/{lessonId:guid}")]
    public async Task<IActionResult> GetQuizByLesson(Guid lessonId)
    {
        var userId = GetUserId();
        var result = await _quizService.GetQuizByLessonAsync(userId, lessonId);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error);
    }

    // POST /api/quiz/{quizId}/attempt
    [HttpPost("{quizId:guid}/attempt")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> SubmitAttempt(Guid quizId, [FromBody] QuizAttemptRequest request)
    {
        var userId = GetUserId();
        var result = await _quizService.SubmitAttemptAsync(userId, quizId, request);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    // GET /api/quiz/{quizId}/attempts/my
    [HttpGet("{quizId:guid}/attempts/my")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyAttempts(Guid quizId)
    {
        var userId = GetUserId();
        var result = await _quizService.GetMyAttemptsAsync(userId, quizId);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    private Guid GetUserId()
        => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}