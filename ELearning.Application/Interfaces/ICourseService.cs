using ELearning.Application.Common;
using ELearning.Application.DTOs.Courses;

namespace ELearning.Application.Interfaces;

public interface ICourseService
{
    // Courses
    Task<Result<IEnumerable<CourseDto>>> GetPublishedCoursesAsync();
    Task<Result<IEnumerable<CourseDto>>> GetTeacherCoursesAsync(Guid teacherId);
    Task<Result<CourseDetailsDto>> GetCourseDetailsAsync(Guid courseId);
    Task<Result<CourseDto>> CreateCourseAsync(Guid teacherId, CreateCourseRequest request);
    Task<Result<CourseDto>> UpdateCourseAsync(Guid courseId, Guid teacherId, UpdateCourseRequest request);
    Task<Result<bool>> PublishCourseAsync(Guid courseId, Guid teacherId);
    Task<Result<bool>> DeleteCourseAsync(Guid courseId, Guid teacherId);

    // Sections
    Task<Result<SectionDto>> AddSectionAsync(Guid courseId, Guid teacherId, CreateSectionRequest request);
    Task<Result<bool>> DeleteSectionAsync(Guid sectionId, Guid teacherId);

    // Lessons
    Task<Result<LessonDto>> AddLessonAsync(Guid sectionId, Guid teacherId, CreateLessonRequest request);
    Task<Result<bool>> DeleteLessonAsync(Guid lessonId, Guid teacherId);
}