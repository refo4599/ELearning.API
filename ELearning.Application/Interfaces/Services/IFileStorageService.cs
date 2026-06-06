using ELearning.Application.Common;
using ELearning.Application.DTOs.Upload;

namespace ELearning.Application.Interfaces;

public interface IFileStorageService
{
    Task<Result<FileUploadResult>> UploadVideoAsync(Stream fileStream, string fileName);
    Task<Result<FileUploadResult>> UploadPdfAsync(Stream fileStream, string fileName);
    Task<Result<FileUploadResult>> UploadThumbnailAsync(Stream fileStream, string fileName);
    Task<Result<bool>> DeleteFileAsync(string publicId);
}