using ELearning.Application.Common;
using ELearning.Application.DTOs.Profile;

namespace ELearning.Application.Interfaces;

public interface IProfileService
{
    Task<Result<StudentProfileDto>> CompleteStudentProfileAsync(
        Guid userId, CompleteStudentProfileRequest request);

    Task<Result<TeacherProfileDto>> CompleteTeacherProfileAsync(
        Guid userId, CompleteTeacherProfileRequest request);

    Task<Result<StudentProfileDto>> GetStudentProfileAsync(Guid userId);
    Task<Result<TeacherProfileDto>> GetTeacherProfileAsync(Guid userId);
}