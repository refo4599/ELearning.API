using ELearning.Application.Common;
using ELearning.Application.DTOs.Courses;
using ELearning.Application.Interfaces;
using ELearning.Domain.Entities;
using ELearning.Domain.Enums;
using ELearning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _uow;
    private readonly AppDbContext _context;

    public CourseService(IUnitOfWork uow, AppDbContext context)
    {
        _uow = uow;
        _context = context;
    }

    public async Task<Result<IEnumerable<CourseDto>>> GetPublishedCoursesAsync()
    {
        var courses = await _uow.Courses.GetPublishedCoursesAsync();
        return Result<IEnumerable<CourseDto>>.Success(courses.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<CourseDto>>> GetTeacherCoursesAsync(Guid teacherId)
    {
        var courses = await _uow.Courses.GetCoursesByTeacherAsync(teacherId);
        return Result<IEnumerable<CourseDto>>.Success(courses.Select(MapToDto));
    }

    public async Task<Result<CourseDetailsDto>> GetCourseDetailsAsync(Guid courseId)
    {
        var course = await _uow.Courses.GetCourseWithDetailsAsync(courseId);
        if (course is null)
            return Result<CourseDetailsDto>.Failure("الكورس غير موجود");

        return Result<CourseDetailsDto>.Success(MapToDetailsDto(course));
    }

    public async Task<Result<CourseDto>> CreateCourseAsync(Guid teacherId, CreateCourseRequest request)
    {
        var category = await _uow.Categories.GetByIdAsync(request.CategoryId);
        if (category is null)
            return Result<CourseDto>.Failure("الفئة غير موجودة");

        if (!Enum.TryParse<CourseLevel>(request.Level, out var level))
            return Result<CourseDto>.Failure("مستوى الكورس غير صحيح");

        var course = new Course
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Level = level,
            Language = request.Language,
            CategoryId = request.CategoryId,
            TeacherId = teacherId,
            Status = CourseStatus.Draft
        };

        await _uow.Courses.AddAsync(course);
        await _uow.SaveChangesAsync();

        // reload مع الـ relations
        var created = await _uow.Courses.GetCourseWithDetailsAsync(course.Id);
        return Result<CourseDto>.Success(MapToDto(created!));
    }

    public async Task<Result<CourseDto>> UpdateCourseAsync(
        Guid courseId, Guid teacherId, UpdateCourseRequest request)
    {
        var course = await _uow.Courses.GetByIdAsync(courseId);
        if (course is null)
            return Result<CourseDto>.Failure("الكورس غير موجود");

        if (course.TeacherId != teacherId)
            return Result<CourseDto>.Failure("غير مصرح لك بتعديل هذا الكورس");

        if (!Enum.TryParse<CourseLevel>(request.Level, out var level))
            return Result<CourseDto>.Failure("مستوى الكورس غير صحيح");

        course.Title = request.Title;
        course.Description = request.Description;
        course.Price = request.Price;
        course.Level = level;
        course.Language = request.Language;
        course.CategoryId = request.CategoryId;
        course.ThumbnailUrl = request.ThumbnailUrl;

        _uow.Courses.Update(course);
        await _uow.SaveChangesAsync();

        var updated = await _uow.Courses.GetCourseWithDetailsAsync(course.Id);
        return Result<CourseDto>.Success(MapToDto(updated!));
    }

    public async Task<Result<bool>> PublishCourseAsync(Guid courseId, Guid teacherId)
    {
        var course = await _uow.Courses.GetCourseWithDetailsAsync(courseId);
        if (course is null)
            return Result<bool>.Failure("الكورس غير موجود");

        if (course.TeacherId != teacherId)
            return Result<bool>.Failure("غير مصرح لك");

        if (!course.Sections.Any())
            return Result<bool>.Failure("لا يمكن نشر كورس بدون sections");

        if (!course.Sections.Any(s => s.Lessons.Any()))
            return Result<bool>.Failure("لا يمكن نشر كورس بدون lessons");

        course.Status = CourseStatus.Published;
        _uow.Courses.Update(course);
        await _uow.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteCourseAsync(Guid courseId, Guid teacherId)
    {
        var course = await _uow.Courses.GetByIdAsync(courseId);
        if (course is null)
            return Result<bool>.Failure("الكورس غير موجود");

        if (course.TeacherId != teacherId)
            return Result<bool>.Failure("غير مصرح لك");

        _uow.Courses.SoftDelete(course);
        await _uow.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<SectionDto>> AddSectionAsync(
        Guid courseId, Guid teacherId, CreateSectionRequest request)
    {
        var course = await _uow.Courses.GetByIdAsync(courseId);
        if (course is null)
            return Result<SectionDto>.Failure("الكورس غير موجود");

        if (course.TeacherId != teacherId)
            return Result<SectionDto>.Failure("غير مصرح لك");

        var section = new Section
        {
            CourseId = courseId,
            Title = request.Title,
            OrderIndex = request.OrderIndex
        };

        _context.Sections.Add(section);
        await _uow.SaveChangesAsync();

        return Result<SectionDto>.Success(new SectionDto
        {
            Id = section.Id,
            Title = section.Title,
            OrderIndex = section.OrderIndex,
            Lessons = []
        });
    }

    public async Task<Result<bool>> DeleteSectionAsync(Guid sectionId, Guid teacherId)
    {
        var section = await _context.Sections
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.Id == sectionId);

        if (section is null)
            return Result<bool>.Failure("الـ Section غير موجود");

        if (section.Course.TeacherId != teacherId)
            return Result<bool>.Failure("غير مصرح لك");

        section.IsDeleted = true;
        await _uow.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<LessonDto>> AddLessonAsync(
        Guid sectionId, Guid teacherId, CreateLessonRequest request)
    {
        var section = await _context.Sections
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.Id == sectionId);

        if (section is null)
            return Result<LessonDto>.Failure("الـ Section غير موجود");

        if (section.Course.TeacherId != teacherId)
            return Result<LessonDto>.Failure("غير مصرح لك");

        var lesson = new Lesson
        {
            SectionId = sectionId,
            Title = request.Title,
            ContentType = request.ContentType,
            OrderIndex = request.OrderIndex,
            DurationSeconds = request.DurationSeconds,
            IsFree = request.IsFree
        };

        _context.Lessons.Add(lesson);
        await _uow.SaveChangesAsync();

        return Result<LessonDto>.Success(new LessonDto
        {
            Id = lesson.Id,
            Title = lesson.Title,
            ContentType = lesson.ContentType.ToString(),
            OrderIndex = lesson.OrderIndex,
            DurationSeconds = lesson.DurationSeconds,
            IsFree = lesson.IsFree
        });
    }

    public async Task<Result<bool>> DeleteLessonAsync(Guid lessonId, Guid teacherId)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Section)
                .ThenInclude(s => s.Course)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson is null)
            return Result<bool>.Failure("الـ Lesson غير موجود");

        if (lesson.Section.Course.TeacherId != teacherId)
            return Result<bool>.Failure("غير مصرح لك");

        lesson.IsDeleted = true;
        await _uow.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    private static CourseDto MapToDto(Course c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Description = c.Description,
        ThumbnailUrl = c.ThumbnailUrl,
        Price = c.Price,
        Status = c.Status.ToString(),
        Level = c.Level.ToString(),
        Language = c.Language,
        AverageRating = c.AverageRating,
        TotalStudents = c.TotalStudents,
        TeacherId = c.TeacherId,
        TeacherName = c.Teacher?.FullName ?? "",
        CategoryId = c.CategoryId,
        CategoryName = c.Category?.Name ?? "",
        CreatedAt = c.CreatedAt
    };

    private static CourseDetailsDto MapToDetailsDto(Course c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Description = c.Description,
        ThumbnailUrl = c.ThumbnailUrl,
        Price = c.Price,
        Status = c.Status.ToString(),
        Level = c.Level.ToString(),
        Language = c.Language,
        AverageRating = c.AverageRating,
        TotalStudents = c.TotalStudents,
        TeacherId = c.TeacherId,
        TeacherName = c.Teacher?.FullName ?? "",
        CategoryId = c.CategoryId,
        CategoryName = c.Category?.Name ?? "",
        CreatedAt = c.CreatedAt,
        Sections = c.Sections.Select(s => new SectionDto
        {
            Id = s.Id,
            Title = s.Title,
            OrderIndex = s.OrderIndex,
            Lessons = s.Lessons.Select(l => new LessonDto
            {
                Id = l.Id,
                Title = l.Title,
                ContentType = l.ContentType.ToString(),
                OrderIndex = l.OrderIndex,
                DurationSeconds = l.DurationSeconds,
                IsFree = l.IsFree
            }).ToList()
        }).ToList()
    };
}