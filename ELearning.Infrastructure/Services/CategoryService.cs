using ELearning.Application.Common;
using ELearning.Application.DTOs.Categories;
using ELearning.Application.Interfaces;
using ELearning.Domain.Entities;

namespace ELearning.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;

    public CategoryService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<IEnumerable<CategoryDto>>> GetAllAsync()
    {
        var categories = await _uow.Categories.GetAllWithChildrenAsync();
        return Result<IEnumerable<CategoryDto>>.Success(
            categories.Select(MapToDto));
    }

    public async Task<Result<CategoryDto>> GetByIdAsync(Guid id)
    {
        var category = await _uow.Categories.GetByIdWithChildrenAsync(id);
        if (category is null)
            return Result<CategoryDto>.Failure("الفئة غير موجودة");

        return Result<CategoryDto>.Success(MapToDto(category));
    }

    public async Task<Result<CategoryDto>> CreateAsync(CreateCategoryRequest request)
    {
        // تأكد إن الاسم مش موجود
        var exists = await _uow.Categories
            .AnyAsync(c => c.Name == request.Name);
        if (exists)
            return Result<CategoryDto>.Failure("هذه الفئة موجودة بالفعل");

        // لو فيه ParentId تأكد إنه موجود
        if (request.ParentId.HasValue)
        {
            var parent = await _uow.Categories.GetByIdAsync(request.ParentId.Value);
            if (parent is null)
                return Result<CategoryDto>.Failure("الفئة الأب غير موجودة");
        }

        var category = new Category
        {
            Name = request.Name,
            IconUrl = request.IconUrl,
            ParentId = request.ParentId
        };

        await _uow.Categories.AddAsync(category);
        await _uow.SaveChangesAsync();

        return Result<CategoryDto>.Success(MapToDto(category));
    }

    public async Task<Result<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await _uow.Categories.GetByIdAsync(id);
        if (category is null)
            return Result<CategoryDto>.Failure("الفئة غير موجودة");

        category.Name = request.Name;
        category.IconUrl = request.IconUrl;

        _uow.Categories.Update(category);
        await _uow.SaveChangesAsync();

        return Result<CategoryDto>.Success(MapToDto(category));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var category = await _uow.Categories.GetByIdWithChildrenAsync(id);
        if (category is null)
            return Result<bool>.Failure("الفئة غير موجودة");

        if (category.Children.Any())
            return Result<bool>.Failure("لا يمكن حذف فئة بها فئات فرعية");

        _uow.Categories.SoftDelete(category);
        await _uow.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    private static CategoryDto MapToDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        IconUrl = c.IconUrl,
        ParentId = c.ParentId,
        Children = c.Children?.Select(MapToDto).ToList() ?? []
    };
}