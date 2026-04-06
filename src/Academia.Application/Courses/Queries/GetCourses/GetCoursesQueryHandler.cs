using Academia.Application.Common.Interfaces;
using Academia.Application.Common.Models;
using Academia.Application.Courses.Dtos;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Courses.Queries.GetCourses;

public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, PagedResult<CourseListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCoursesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<CourseListDto>> Handle(
        GetCoursesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Chapters)
                .ThenInclude(ch => ch.Lessons)
            .AsNoTracking()
            .AsQueryable();

        if (request.PublicOnly)
            query = query.Where(c => c.Status == CourseStatus.Published);

        if (request.Status.HasValue)
            query = query.Where(c => c.Status == request.Status.Value);

        if (request.AccessType.HasValue)
            query = query.Where(c => c.AccessType == request.AccessType.Value);

        if (request.TeacherId.HasValue)
            query = query.Where(c => c.TeacherId == request.TeacherId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(c =>
                c.Title.ToLower().Contains(term) ||
                (c.Description != null && c.Description.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var courses = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = courses.Select(c => new CourseListDto(
            Id: c.Id,
            Title: c.Title,
            Description: c.Description,
            Image: c.Image,
            Status: c.Status,
            AccessType: c.AccessType,
            Price: c.Price,
            EstimatedDuration: c.EstimatedDuration,
            Teacher: c.Teacher is null ? null
                : new TeacherSummaryDto(c.Teacher.Id, c.Teacher.FullName, c.Teacher.Avatar),
            ChapterCount: c.Chapters.Count,
            LessonCount: c.Chapters.Sum(ch => ch.Lessons.Count),
            CreatedAt: c.CreatedAt
        )).ToList();

        return new PagedResult<CourseListDto>(items, totalCount, request.Page, request.PageSize);
    }
}
