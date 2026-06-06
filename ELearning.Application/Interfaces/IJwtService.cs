using ELearning.Domain.Entities;

namespace ELearning.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}