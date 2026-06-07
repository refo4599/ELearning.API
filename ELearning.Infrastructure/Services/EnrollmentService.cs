using ELearning.Application.Common;
using ELearning.Application.DTOs.Enrollments;
using ELearning.Application.Interfaces;
using ELearning.Domain.Entities;
using ELearning.Domain.Enums;

namespace ELearning.Infrastructure.Services;
public class EnrollmentService : IEnrollmentService
{
    private readonly IUnitOfWork _uow;
    public EnrollmentService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<EnrollmentDto>> EnrollAsync(Guid userId, Guid courseId)
    {
        var course = await _uow.Courses.GetByIdAsync(courseId);
        if (course is null || course.Status != CourseStatus.Published)
            return Result<EnrollmentDto>.Failure("الكورس غير متاح.");

        var alreadyEnrolled = await _uow.Enrollments.IsEnrolledAsync(userId, courseId);
        if (alreadyEnrolled)
            return Result<EnrollmentDto>.Failure("أنت مشترك في هذا الكورس بالفعل.");

        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow,
            CompletionPercentage = 0
        };

        await _uow.Enrollments.AddAsync(enrollment);
        await _uow.SaveChangesAsync();

        return Result<EnrollmentDto>.Success(MapToDto(enrollment, course));
    }

    public async Task<Result<IEnumerable<EnrollmentDto>>> GetMyEnrollmentsAsync(Guid userId)
    {
        var enrollments = await _uow.Enrollments.GetUserEnrollmentsAsync(userId);
        var dtos = enrollments.Select(e => MapToDto(e, e.Course));
        return Result<IEnumerable<EnrollmentDto>>.Success(dtos);
    }

    public async Task<Result<bool>> UnenrollAsync(Guid userId, Guid courseId)
    {
        var enrollment = await _uow.Enrollments.GetByUserAndCourseAsync(userId, courseId);
        if (enrollment is null)
            return Result<bool>.Failure("أنت لست مشتركاً في هذا الكورس.");

        _uow.Enrollments.Delete(enrollment);   // void — مش await
        await _uow.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    private static EnrollmentDto MapToDto(Enrollment e, Course course) => new()
    {
        Id = e.Id,
        CourseId = course.Id,
        CourseTitle = course.Title,
        CourseThumbnailUrl = course.ThumbnailUrl,
        TeacherName = course.Teacher?.FullName ?? string.Empty,  // FullName مش FirstName+LastName
        EnrolledAt = e.EnrolledAt,
        CompletionPercentage = e.CompletionPercentage,
        IsCompleted = e.CompletionPercentage >= 100
    };
}