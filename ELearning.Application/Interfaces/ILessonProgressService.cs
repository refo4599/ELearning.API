using ELearning.Application.Common;
using ELearning.Application.DTOs.Progress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ELearning.Application.Interfaces;

public interface ILessonProgressService
{
    Task<Result<LessonProgressDto>> MarkLessonCompleteAsync(Guid studentId, Guid lessonId);
    Task<Result<CourseProgressDto>> GetCourseProgressAsync(Guid studentId, Guid courseId);
}