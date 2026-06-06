using ELearning.Domain.Entities;

namespace ELearning.Application.Interfaces.Repositories;

public interface ICourseRepository : IGenericRepository<Course>
{
    Task<Course?> GetCourseWithDetailsAsync(Guid courseId);
    Task<IEnumerable<Course>> GetCoursesByTeacherAsync(Guid teacherId);
    Task<IEnumerable<Course>> GetPublishedCoursesAsync();
}