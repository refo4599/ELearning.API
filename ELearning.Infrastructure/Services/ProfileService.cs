using System.Text.Json;
using ELearning.Application.Common;
using ELearning.Application.DTOs.Profile;
using ELearning.Application.Interfaces;
using ELearning.Domain.Entities;
using ELearning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Services;

public class ProfileService : IProfileService
{
    private readonly AppDbContext _context;

    public ProfileService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StudentProfileDto>> CompleteStudentProfileAsync(
        Guid userId, CompleteStudentProfileRequest request)
    {
        var user = await _context.Users
            .Include(u => u.StudentProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return Result<StudentProfileDto>.Failure("المستخدم غير موجود");

        // تحديث اسم اليوزر
        user.FullName = request.FullName;
        user.IsProfileComplete = true;

        if (user.StudentProfile is null)
        {
            // إنشاء profile جديد
            user.StudentProfile = new StudentProfile
            {
                UserId = userId,
                Age = request.Age,
                GradeLevel = request.GradeLevel,
                School = request.School,
                Address = request.Address,
                ParentPhone = request.ParentPhone,
                PreferredSubjects = JsonSerializer.Serialize(request.PreferredSubjects)
            };
            _context.StudentProfiles.Add(user.StudentProfile);
        }
        else
        {
            // تحديث profile موجود
            user.StudentProfile.Age = request.Age;
            user.StudentProfile.GradeLevel = request.GradeLevel;
            user.StudentProfile.School = request.School;
            user.StudentProfile.Address = request.Address;
            user.StudentProfile.ParentPhone = request.ParentPhone;
            user.StudentProfile.PreferredSubjects =
                JsonSerializer.Serialize(request.PreferredSubjects);
        }

        await _context.SaveChangesAsync();

        return Result<StudentProfileDto>.Success(MapToStudentDto(user));
    }

    public async Task<Result<TeacherProfileDto>> CompleteTeacherProfileAsync(
        Guid userId, CompleteTeacherProfileRequest request)
    {
        var user = await _context.Users
            .Include(u => u.TeacherProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return Result<TeacherProfileDto>.Failure("المستخدم غير موجود");

        user.FullName = request.FullName;
        user.IsProfileComplete = true;

        if (user.TeacherProfile is null)
        {
            user.TeacherProfile = new TeacherProfile
            {
                UserId = userId,
                Specialization = request.Specialization,
                Bio = request.Bio,
                PhoneNumber = request.PhoneNumber,
                Qualifications = request.Qualifications,
                BankAccountNumber = request.BankAccountNumber,
                BankName = request.BankName
            };
            _context.TeacherProfiles.Add(user.TeacherProfile);
        }
        else
        {
            user.TeacherProfile.Specialization = request.Specialization;
            user.TeacherProfile.Bio = request.Bio;
            user.TeacherProfile.PhoneNumber = request.PhoneNumber;
            user.TeacherProfile.Qualifications = request.Qualifications;
            user.TeacherProfile.BankAccountNumber = request.BankAccountNumber;
            user.TeacherProfile.BankName = request.BankName;
        }

        await _context.SaveChangesAsync();

        return Result<TeacherProfileDto>.Success(MapToTeacherDto(user));
    }

    public async Task<Result<StudentProfileDto>> GetStudentProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.StudentProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return Result<StudentProfileDto>.Failure("المستخدم غير موجود");

        return Result<StudentProfileDto>.Success(MapToStudentDto(user));
    }

    public async Task<Result<TeacherProfileDto>> GetTeacherProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.TeacherProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return Result<TeacherProfileDto>.Failure("المستخدم غير موجود");

        return Result<TeacherProfileDto>.Success(MapToTeacherDto(user));
    }

    private static StudentProfileDto MapToStudentDto(User user) => new()
    {
        UserId = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        AvatarUrl = user.AvatarUrl,
        IsProfileComplete = user.IsProfileComplete,
        Age = user.StudentProfile?.Age ?? 0,
        GradeLevel = user.StudentProfile?.GradeLevel ?? "",
        School = user.StudentProfile?.School ?? "",
        Address = user.StudentProfile?.Address ?? "",
        ParentPhone = user.StudentProfile?.ParentPhone ?? "",
        PreferredSubjects = user.StudentProfile?.PreferredSubjects is not null
            ? JsonSerializer.Deserialize<List<string>>(
                user.StudentProfile.PreferredSubjects) ?? []
            : []
    };

    private static TeacherProfileDto MapToTeacherDto(User user) => new()
    {
        UserId = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        AvatarUrl = user.AvatarUrl,
        IsProfileComplete = user.IsProfileComplete,
        Specialization = user.TeacherProfile?.Specialization ?? "",
        Bio = user.TeacherProfile?.Bio ?? "",
        PhoneNumber = user.TeacherProfile?.PhoneNumber ?? "",
        Qualifications = user.TeacherProfile?.Qualifications ?? "",
        BankAccountNumber = user.TeacherProfile?.BankAccountNumber,
        BankName = user.TeacherProfile?.BankName,
        IsVerified = user.TeacherProfile?.IsVerified ?? false
    };
}