using ELearning.Domain.Common;
using ELearning.Domain.Enums;

namespace ELearning.Domain.Entities;

public class Lesson : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public ContentType ContentType { get; set; }
    public int OrderIndex { get; set; }
    public int DurationSeconds { get; set; }
    public bool IsFree { get; set; } = false;
    public Guid SectionId { get; set; }

    // Navigation Properties
    public Section Section { get; set; } = null!;
    public VideoContent? VideoContent { get; set; }
    public PdfContent? PdfContent { get; set; }
    public Quiz? Quiz { get; set; }
    public ICollection<LessonProgress> Progresses { get; set; } = [];
}