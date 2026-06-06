using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// يُرسَل بعد الرفع لربط الملف بالـ Lesson
public class AttachVideoRequest
{
    public string PublicId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
}

public class AttachPdfRequest
{
    public string PublicId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int PageCount { get; set; }
}