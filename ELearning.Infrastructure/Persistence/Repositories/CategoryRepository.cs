using ELearning.Application.Interfaces.Repositories;
using ELearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Category>> GetAllWithChildrenAsync()
        => await _dbSet
            .Include(c => c.Children)
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<Category?> GetByIdWithChildrenAsync(Guid id)
        => await _dbSet
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Id == id);
}