using ELearning.Application.Interfaces;
using ELearning.Domain.Entities;
using ELearning.Infrastructure.Persistence;
using ELearning.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Repositories;

public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(AppDbContext context) : base(context) { }

    public async Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId)
        => await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

    public async Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(Guid userId)
        => await _context.Enrollments
            .Include(e => e.Course)
                .ThenInclude(c => c.Teacher)
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();

    public async Task<bool> IsEnrolledAsync(Guid userId, Guid courseId)
        => await _context.Enrollments
            .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
}