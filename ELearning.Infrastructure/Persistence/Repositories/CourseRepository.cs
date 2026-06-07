using ELearning.Application.Interfaces.Repositories;
using ELearning.Domain.Entities;
using ELearning.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Persistence.Repositories;

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(AppDbContext context) : base(context) { }

    public async Task<Course?> GetCourseWithDetailsAsync(Guid courseId)
        => await _dbSet
            .Include(c => c.Teacher)
            .Include(c => c.Category)
            .Include(c => c.Sections.OrderBy(s => s.OrderIndex))
                .ThenInclude(s => s.Lessons.OrderBy(l => l.OrderIndex))
            .FirstOrDefaultAsync(c => c.Id == courseId);

    public async Task<IEnumerable<Course>> GetCoursesByTeacherAsync(Guid teacherId)
        => await _dbSet
            .Include(c => c.Category)
            .Where(c => c.TeacherId == teacherId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Course>> GetPublishedCoursesAsync()
        => await _dbSet
            .Include(c => c.Teacher)
            .Include(c => c.Category)
            .Where(c => c.Status == CourseStatus.Published)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
}