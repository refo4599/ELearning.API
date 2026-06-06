using ELearning.Domain.Common;

namespace ELearning.Domain.Entities;

public class PdfContent : BaseEntity
{
    public string PdfUrl { get; set; } = string.Empty;     
    public string FileName { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public long FileSizeBytes { get; set; }
    public long FileSizeInBytes { get; set; }
    public string CloudinaryPublicId { get; set; } = string.Empty;
    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
}