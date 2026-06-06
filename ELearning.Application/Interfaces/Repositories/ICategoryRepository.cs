using ELearning.Domain.Entities;

namespace ELearning.Application.Interfaces.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetAllWithChildrenAsync();
    Task<Category?> GetByIdWithChildrenAsync(Guid id);
}