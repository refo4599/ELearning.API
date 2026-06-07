using ELearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearning.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(e => e.Id);

        // منع التسجيل المكرر في نفس الكورس
        builder.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();

        builder.Property(e => e.PaidAmount)
               .HasColumnType("decimal(18,2)");

        builder.Property(e => e.PaymentStatus)
               .HasConversion<string>();
    }
}