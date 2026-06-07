using ELearning.Domain.Entities;
using ELearning.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearning.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(150);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Role)
               .HasConversion<string>();

        builder.HasMany(u => u.TaughtCourses)
               .WithOne(c => c.Teacher)
               .HasForeignKey(c => c.TeacherId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.HostedSessions)
               .WithOne(s => s.Teacher)
               .HasForeignKey(s => s.TeacherId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}