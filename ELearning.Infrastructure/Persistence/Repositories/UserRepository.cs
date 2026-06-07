using ELearning.Application.Interfaces.Repositories;
using ELearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
        => await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        => await _dbSet.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
}