using ELearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ELearning.Infrastructure.Persistence.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.ContentType)
               .HasConversion<string>();

        builder.HasOne(l => l.VideoContent)
               .WithOne(v => v.Lesson)
               .HasForeignKey<VideoContent>(v => v.LessonId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.PdfContent)
               .WithOne(p => p.Lesson)
               .HasForeignKey<PdfContent>(p => p.LessonId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Quiz)
               .WithOne(q => q.Lesson)
               .HasForeignKey<Quiz>(q => q.LessonId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}