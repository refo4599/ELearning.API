using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ELearning.Application.Common;
using ELearning.Application.DTOs.Upload;
using ELearning.Application.Interfaces;
using ELearning.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace ELearning.Infrastructure.Services;

public class CloudinaryService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;

    private static readonly HashSet<string> _videoExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".mp4", ".mov", ".avi", ".mkv", ".webm" };

    private static readonly HashSet<string> _pdfExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".pdf" };

    private static readonly HashSet<string> _imageExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    private const long MaxVideoSize = 500 * 1024 * 1024;
    private const long MaxPdfSize = 50 * 1024 * 1024;
    private const long MaxThumbnailSize = 5 * 1024 * 1024;

    public CloudinaryService(IOptions<CloudinarySettings> settings)
    {
        var s = settings.Value;
        var account = new Account(s.CloudName, s.ApiKey, s.ApiSecret);
        _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
    }

    public async Task<Result<FileUploadResult>> UploadVideoAsync(Stream fileStream, string fileName)
    {
        var validation = ValidateFile(fileName, fileStream.Length, _videoExtensions, MaxVideoSize, "فيديو");
        if (!validation.IsSuccess) return Result<FileUploadResult>.Failure(validation.Error!);

        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = "elearning/videos",
            // ← الطريقة الصح في Cloudinary SDK — transformation على الـ upload مش Eager
            Transformation = new Transformation().VideoCodec("h264").AudioCodec("aac").Quality(70)
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
            return Result<FileUploadResult>.Failure($"فشل الرفع: {result.Error.Message}");

        return Result<FileUploadResult>.Success(new FileUploadResult
        {
            PublicId = result.PublicId,
            Url = result.SecureUrl.ToString(),
            Format = result.Format,
            Bytes = result.Bytes,
            DurationSeconds = (int?)result.Duration,
            Width = result.Width,
            Height = result.Height
        });
    }

    public async Task<Result<FileUploadResult>> UploadPdfAsync(Stream fileStream, string fileName)
    {
        var validation = ValidateFile(fileName, fileStream.Length, _pdfExtensions, MaxPdfSize, "PDF");
        if (!validation.IsSuccess) return Result<FileUploadResult>.Failure(validation.Error!);

        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = "elearning/pdfs"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
            return Result<FileUploadResult>.Failure($"فشل الرفع: {result.Error.Message}");

        return Result<FileUploadResult>.Success(new FileUploadResult
        {
            PublicId = result.PublicId,
            Url = result.SecureUrl.ToString(),
            Format = "pdf",
            Bytes = result.Bytes
        });
    }

    public async Task<Result<FileUploadResult>> UploadThumbnailAsync(Stream fileStream, string fileName)
    {
        var validation = ValidateFile(fileName, fileStream.Length, _imageExtensions, MaxThumbnailSize, "صورة");
        if (!validation.IsSuccess) return Result<FileUploadResult>.Failure(validation.Error!);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = "elearning/thumbnails",
            Transformation = new Transformation().Width(1280).Height(720).Crop("fill").Quality(80),
            Format = "webp"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
            return Result<FileUploadResult>.Failure($"فشل الرفع: {result.Error.Message}");

        return Result<FileUploadResult>.Success(new FileUploadResult
        {
            PublicId = result.PublicId,
            Url = result.SecureUrl.ToString(),
            Format = result.Format,
            Bytes = result.Bytes,
            Width = result.Width,
            Height = result.Height
        });
    }

    public async Task<Result<bool>> DeleteFileAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);

        if (publicId.Contains("videos"))
            deletionParams.ResourceType = ResourceType.Video;

        var result = await _cloudinary.DestroyAsync(deletionParams);

        return result.Result == "ok"
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("فشل حذف الملف من Cloudinary");
    }

    // ← Stream مش بيدينا Length مباشرة لو كان non-seekable
    // بس IFormFile.OpenReadStream() بيدي seekable stream فـ Length شغالة
    private static Result<bool> ValidateFile(
        string fileName,
        long fileSize,
        HashSet<string> allowedExtensions,
        long maxSize,
        string fileType)
    {
        if (string.IsNullOrEmpty(fileName) || fileSize == 0)
            return Result<bool>.Failure("الملف فارغ");

        var ext = Path.GetExtension(fileName);
        if (!allowedExtensions.Contains(ext))
            return Result<bool>.Failure(
                $"امتداد غير مسموح به. المسموح: {string.Join(", ", allowedExtensions)}");

        if (fileSize > maxSize)
            return Result<bool>.Failure(
                $"حجم الـ {fileType} يتجاوز الحد ({maxSize / 1024 / 1024} MB)");

        return Result<bool>.Success(true);
    }
}