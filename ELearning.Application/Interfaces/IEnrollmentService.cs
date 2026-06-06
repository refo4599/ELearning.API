using ELearning.Application.Common;
using ELearning.Application.DTOs.Enrollments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IEnrollmentService
{
    Task<Result<EnrollmentDto>> EnrollAsync(Guid userId, Guid courseId);
    Task<Result<IEnumerable<EnrollmentDto>>> GetMyEnrollmentsAsync(Guid userId);
    Task<Result<bool>> UnenrollAsync(Guid userId, Guid courseId);
}