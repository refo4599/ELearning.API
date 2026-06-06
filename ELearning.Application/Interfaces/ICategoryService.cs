using ELearning.Application.Common;
using ELearning.Application.DTOs.Categories;

namespace ELearning.Application.Interfaces;

public interface ICategoryService
{
    Task<Result<IEnumerable<CategoryDto>>> GetAllAsync();
    Task<Result<CategoryDto>> GetByIdAsync(Guid id);
    Task<Result<CategoryDto>> CreateAsync(CreateCategoryRequest request);
    Task<Result<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
}