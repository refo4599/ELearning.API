using ELearning.Application.Interfaces;
using ELearning.Domain.Entities;
using ELearning.Infrastructure.Persistence;
using ELearning.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Repositories;

public class LessonProgressRepository : GenericRepository<LessonProgress>, ILessonProgressRepository
{
    public LessonProgressRepository(AppDbContext context) : base(context) { }

    public async Task<LessonProgress?> GetByUserAndLessonAsync(Guid userId, Guid lessonId)
        => await _context.LessonProgresses
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);

    public async Task<IEnumerable<LessonProgress>> GetUserProgressInCourseAsync(Guid userId, Guid courseId)
        => await _context.LessonProgresses
            .Include(p => p.Lesson)
            .Where(p => p.UserId == userId && p.Lesson.Section.CourseId == courseId)
            .ToListAsync();

    public async Task<int> CountCompletedLessonsAsync(Guid userId, Guid courseId)
        => await _context.LessonProgresses
            .CountAsync(p =>
                p.UserId == userId &&
                p.IsCompleted &&
                p.Lesson.Section.CourseId == courseId);
}