using ELearning.Domain.Entities;

namespace ELearning.Application.Interfaces.Repositories;

public interface ILessonRepository : IGenericRepository<Lesson>
{
    Task<Lesson?> GetWithSectionAsync(Guid lessonId);
}