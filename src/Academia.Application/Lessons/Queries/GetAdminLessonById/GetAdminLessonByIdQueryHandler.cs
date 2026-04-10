using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Courses.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Lessons.Queries.GetAdminLessonById;

public class GetAdminLessonByIdQueryHandler : IRequestHandler<GetAdminLessonByIdQuery, AdminLessonDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetAdminLessonByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<AdminLessonDetailDto> Handle(
        GetAdminLessonByIdQuery request, CancellationToken cancellationToken)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Chapter)
                .ThenInclude(ch => ch.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.LessonId, cancellationToken);

        if (lesson is null)
            throw new NotFoundException("Lesson", request.LessonId);

        var attachments = await _context.LessonAttachments
            .Where(a => a.LessonId == lesson.Id)
            .OrderBy(a => a.Order)
            .Select(a => new AttachmentDto(a.Id, a.FileName, a.FileUrl, a.FileType, a.FileSize, a.Order))
            .ToListAsync(cancellationToken);

        return new AdminLessonDetailDto(
            Id: lesson.Id,
            ChapterId: lesson.ChapterId,
            ChapterTitle: lesson.Chapter.Title,
            CourseId: lesson.Chapter.CourseId,
            CourseTitle: lesson.Chapter.Course.Title,
            Title: lesson.Title,
            Type: lesson.Type,
            TextContent: lesson.TextContent,
            VideoUrl: lesson.VideoUrl,
            AudioUrl: lesson.AudioUrl,
            PdfFile: lesson.PdfFile,
            DurationMinutes: lesson.DurationMinutes,
            Order: lesson.Order,
            RequiresCompletingPrevious: lesson.RequiresCompletingPrevious,
            AvailableFrom: lesson.AvailableFrom,
            Attachments: attachments,
            CreatedAt: lesson.CreatedAt,
            UpdatedAt: lesson.UpdatedAt
        );
    }
}
