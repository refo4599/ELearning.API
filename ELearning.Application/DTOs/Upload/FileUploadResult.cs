namespace ELearning.Application.DTOs.Upload;

public class FileUploadResult
{
    public string PublicId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public long Bytes { get; set; }
    public int? DurationSeconds { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
}