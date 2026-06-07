using ELearning.Application.Common;
using ELearning.Application.DTOs.Auth;
using ELearning.Application.Interfaces;
using ELearning.Domain.Entities;
using ELearning.Domain.Enums;
using Microsoft.Extensions.Options;

namespace ELearning.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _settings;

    public AuthService(IUnitOfWork uow, IJwtService jwtService,
        IOptions<JwtSettings> settings)
    {
        _uow = uow;
        _jwtService = jwtService;
        _settings = settings.Value;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var exists = await _uow.Users.AnyAsync(u => u.Email == request.Email);
        if (exists)
            return Result<AuthResponse>.Failure("البريد الإلكتروني مستخدم بالفعل");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Enum.TryParse<UserRole>(request.Role, out var role) ? role : UserRole.Student,
            RefreshToken = _jwtService.GenerateRefreshToken(),
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays)
        };

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        return Result<AuthResponse>.Success(BuildAuthResponse(user));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _uow.Users.GetByEmailAsync(request.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("بيانات الدخول غير صحيحة");

        if (!user.IsActive)
            return Result<AuthResponse>.Failure("الحساب موقوف");

        user.RefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays);
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync();

        return Result<AuthResponse>.Success(BuildAuthResponse(user));
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken)
    {
        var user = await _uow.Users.GetByRefreshTokenAsync(refreshToken);

        if (user is null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return Result<AuthResponse>.Failure("Refresh token غير صالح أو منتهي");

        user.RefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays);
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync();

        return Result<AuthResponse>.Success(BuildAuthResponse(user));
    }

    private AuthResponse BuildAuthResponse(User user) => new()
    {
        AccessToken = _jwtService.GenerateAccessToken(user),
        RefreshToken = user.RefreshToken!,
        AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiryMinutes),
        User = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            AvatarUrl = user.AvatarUrl,
            IsProfileComplete = user.IsProfileComplete  // ← جديد
        }
    };
}