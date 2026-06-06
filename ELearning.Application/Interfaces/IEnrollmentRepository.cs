using ELearning.Application.Interfaces.Repositories;
using ELearning.Domain.Entities;

public interface IEnrollmentRepository : IGenericRepository<Enrollment>
{
    Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId);
    Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(Guid userId);
    Task<bool> IsEnrolledAsync(Guid userId, Guid courseId);
}