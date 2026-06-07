using ELearning.Application.Common;
using ELearning.Application.DTOs.Progress;
using ELearning.Application.Interfaces;
using ELearning.Domain.Entities;

namespace ELearning.Infrastructure.Services;

public class LessonProgressService : ILessonProgressService
{
    private readonly IUnitOfWork _uow;

    public LessonProgressService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<LessonProgressDto>> MarkLessonCompleteAsync(Guid userId, Guid lessonId)
    {
        // GetWithSectionAsync بيعمل Include للـ Section فمفيش NullReferenceException
        var lesson = await _uow.Lessons.GetWithSectionAsync(lessonId);
        if (lesson is null)
            return Result<LessonProgressDto>.Failure("الدرس غير موجود.");

        var courseId = lesson.Section.CourseId;

        var isEnrolled = await _uow.Enrollments.IsEnrolledAsync(userId, courseId);
        if (!isEnrolled)
            return Result<LessonProgressDto>.Failure("يجب الاشتراك في الكورس أولاً.");

        var progress = await _uow.LessonProgresses.GetByUserAndLessonAsync(userId, lessonId);
        if (progress is null)
        {
            progress = new LessonProgress
            {
                UserId = userId,
                LessonId = lessonId,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow
            };
            await _uow.LessonProgresses.AddAsync(progress);
        }
        else if (!progress.IsCompleted)
        {
            progress.IsCompleted = true;
            progress.CompletedAt = DateTime.UtcNow;
            _uow.LessonProgresses.Update(progress);
        }

        await UpdateEnrollmentProgressAsync(userId, courseId);
        await _uow.SaveChangesAsync();

        return Result<LessonProgressDto>.Success(new LessonProgressDto
        {
            LessonId = lessonId,
            LessonTitle = lesson.Title,
            IsCompleted = true,
            CompletedAt = progress.CompletedAt
        });
    }

    public async Task<Result<CourseProgressDto>> GetCourseProgressAsync(Guid userId, Guid courseId)
    {
        var enrollment = await _uow.Enrollments.GetByUserAndCourseAsync(userId, courseId);
        if (enrollment is null)
            return Result<CourseProgressDto>.Failure("أنت غير مشترك في هذا الكورس.");

        var course = await _uow.Courses.GetCourseWithDetailsAsync(courseId);
        if (course is null)
            return Result<CourseProgressDto>.Failure("الكورس غير موجود.");

        var allLessons = course.Sections
            .OrderBy(s => s.OrderIndex)
            .SelectMany(s => s.Lessons.OrderBy(l => l.OrderIndex))
            .ToList();

        var progressRecords = await _uow.LessonProgresses.GetUserProgressInCourseAsync(userId, courseId);
        var progressMap = progressRecords.ToDictionary(p => p.LessonId);

        var lessonsProgress = allLessons.Select(l => new LessonProgressDto
        {
            LessonId = l.Id,
            LessonTitle = l.Title,
            IsCompleted = progressMap.TryGetValue(l.Id, out var p) && p.IsCompleted,
            CompletedAt = progressMap.TryGetValue(l.Id, out var p2) ? p2.CompletedAt : null
        });

        return Result<CourseProgressDto>.Success(new CourseProgressDto
        {
            CourseId = courseId,
            CourseTitle = course.Title,
            CompletionPercentage = enrollment.CompletionPercentage,
            TotalLessons = allLessons.Count,
            CompletedLessons = progressMap.Values.Count(p => p.IsCompleted),
            IsCourseCompleted = enrollment.CompletionPercentage >= 100,
            LessonsProgress = lessonsProgress
        });
    }

    private async Task UpdateEnrollmentProgressAsync(Guid userId, Guid courseId)
    {
        var enrollment = await _uow.Enrollments.GetByUserAndCourseAsync(userId, courseId);
        if (enrollment is null) return;

        var course = await _uow.Courses.GetCourseWithDetailsAsync(courseId);
        if (course is null) return;

        var totalLessons = course.Sections.SelectMany(s => s.Lessons).Count();
        if (totalLessons == 0) return;

        var completedLessons = await _uow.LessonProgresses.CountCompletedLessonsAsync(userId, courseId);

        enrollment.CompletionPercentage = (float)Math.Round((double)completedLessons / totalLessons * 100, 2);
        _uow.Enrollments.Update(enrollment);
    }
}