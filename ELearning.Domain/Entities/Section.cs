using ELearning.Domain.Common;

namespace ELearning.Domain.Entities;

public class Section : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public Guid CourseId { get; set; }

    // Navigation Properties
    public Course Course { get; set; } = null!;
    public ICollection<Lesson> Lessons { get; set; } = [];
}