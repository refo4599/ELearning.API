using ELearning.Application.Interfaces.Repositories;
using ELearning.Domain.Entities;

namespace ELearning.Application.Interfaces;

public interface ILessonProgressRepository : IGenericRepository<LessonProgress>
{
    Task<LessonProgress?> GetByUserAndLessonAsync(Guid userId, Guid lessonId);
    Task<IEnumerable<LessonProgress>> GetUserProgressInCourseAsync(Guid userId, Guid courseId);
    Task<int> CountCompletedLessonsAsync(Guid userId, Guid courseId);
}