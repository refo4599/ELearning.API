using ELearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearning.Infrastructure.Persistence.Configurations;

public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasIndex(s => s.UserId).IsUnique();

        builder.Property(s => s.GradeLevel).HasMaxLength(50);
        builder.Property(s => s.School).HasMaxLength(200);
        builder.Property(s => s.Address).HasMaxLength(300);
        builder.Property(s => s.ParentPhone).HasMaxLength(20);
        builder.Property(s => s.PreferredSubjects).HasMaxLength(500);

        builder.HasOne(s => s.User)
               .WithOne(u => u.StudentProfile)
               .HasForeignKey<StudentProfile>(s => s.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}