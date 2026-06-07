using ELearning.Application.Interfaces;
using ELearning.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELearning.API.Controllers;

[ApiController]
[Route("api/upload")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IFileStorageService _storage;
    private readonly IUnitOfWork _uow;

    public UploadController(IFileStorageService storage, IUnitOfWork uow)
    {
        _storage = storage;
        _uow = uow;
    }

    // POST /api/upload/video
    [HttpPost("video")]
    [Authorize(Roles = "Teacher,Admin")]
    [RequestSizeLimit(524_288_000)]
    public async Task<IActionResult> UploadVideo(IFormFile file)
    {
        var result = await _storage.UploadVideoAsync(file.OpenReadStream(), file.FileName);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    // POST /api/upload/pdf
    [HttpPost("pdf")]
    [Authorize(Roles = "Teacher,Admin")]
    [RequestSizeLimit(52_428_800)]
    public async Task<IActionResult> UploadPdf(IFormFile file)
    {
        var result = await _storage.UploadPdfAsync(file.OpenReadStream(), file.FileName);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    // POST /api/upload/thumbnail
    [HttpPost("thumbnail")]
    [Authorize(Roles = "Teacher,Admin")]
    [RequestSizeLimit(5_242_880)]
    public async Task<IActionResult> UploadThumbnail(IFormFile file)
    {
        var result = await _storage.UploadThumbnailAsync(file.OpenReadStream(), file.FileName);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    // POST /api/upload/lesson/{lessonId}/video
    [HttpPost("lesson/{lessonId}/video")]
    [Authorize(Roles = "Teacher,Admin")]
    [RequestSizeLimit(524_288_000)]
    public async Task<IActionResult> UploadAndAttachVideo(Guid lessonId, IFormFile file)
    {
        var lesson = await _uow.Lessons.GetByIdAsync(lessonId);
        if (lesson is null) return NotFound("الدرس غير موجود");

        var uploadResult = await _storage.UploadVideoAsync(file.OpenReadStream(), file.FileName);
        if (!uploadResult.IsSuccess) return BadRequest(uploadResult.Error);

        if (lesson.VideoContent is not null &&
            !string.IsNullOrEmpty(lesson.VideoContent.CloudinaryPublicId))
        {
            await _storage.DeleteFileAsync(lesson.VideoContent.CloudinaryPublicId);
        }

        lesson.VideoContent = new VideoContent
        {
            LessonId = lessonId,
            VideoUrl = uploadResult.Value!.Url,
            CloudinaryPublicId = uploadResult.Value.PublicId,
            DurationInSeconds = uploadResult.Value.DurationSeconds ?? 0,
            Resolution = $"{uploadResult.Value.Width}x{uploadResult.Value.Height}"
        };

        _uow.Lessons.Update(lesson);
        await _uow.SaveChangesAsync();

        return Ok(uploadResult.Value);
    }

    // POST /api/upload/lesson/{lessonId}/pdf
    [HttpPost("lesson/{lessonId}/pdf")]
    [Authorize(Roles = "Teacher,Admin")]
    [RequestSizeLimit(52_428_800)]
    public async Task<IActionResult> UploadAndAttachPdf(Guid lessonId, IFormFile file)
    {
        var lesson = await _uow.Lessons.GetByIdAsync(lessonId);
        if (lesson is null) return NotFound("الدرس غير موجود");

        var uploadResult = await _storage.UploadPdfAsync(file.OpenReadStream(), file.FileName);
        if (!uploadResult.IsSuccess) return BadRequest(uploadResult.Error);

        if (lesson.PdfContent is not null &&
            !string.IsNullOrEmpty(lesson.PdfContent.CloudinaryPublicId))
        {
            await _storage.DeleteFileAsync(lesson.PdfContent.CloudinaryPublicId);
        }

        lesson.PdfContent = new PdfContent
        {
            LessonId = lessonId,
            PdfUrl = uploadResult.Value!.Url,
            CloudinaryPublicId = uploadResult.Value.PublicId,
            FileSizeInBytes = uploadResult.Value.Bytes
        };

        _uow.Lessons.Update(lesson);
        await _uow.SaveChangesAsync();

        return Ok(uploadResult.Value);
    }
}