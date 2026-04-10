using System.Text.Json;
using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Courses.Commands.RollbackCourse;

public class RollbackCourseCommandHandler : IRequestHandler<RollbackCourseCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public RollbackCourseCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(RollbackCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
            throw new NotFoundException("Course", request.CourseId);

        var version = await _context.CourseVersions
            .FirstOrDefaultAsync(v => v.Id == request.VersionId && v.CourseId == request.CourseId,
                cancellationToken);

        if (version is null)
            throw new NotFoundException("CourseVersion", request.VersionId);

        // Deserialize the snapshot to restore top-level course fields
        using var doc = JsonDocument.Parse(version.SnapshotJson);
        var root = doc.RootElement;

        var title = root.GetProperty("Title").GetString()!;
        var description = root.TryGetProperty("Description", out var descProp) ? descProp.GetString() : null;
        var image = root.TryGetProperty("Image", out var imgProp) ? imgProp.GetString() : null;
        var accessTypeStr = root.GetProperty("AccessType").GetString()!;
        var accessType = Enum.Parse<AccessType>(accessTypeStr);
        var price = root.TryGetProperty("Price", out var priceProp) && priceProp.ValueKind != JsonValueKind.Null
            ? priceProp.GetDecimal() : (decimal?)null;
        var duration = root.TryGetProperty("EstimatedDuration", out var durProp) && durProp.ValueKind != JsonValueKind.Null
            ? durProp.GetInt32() : (int?)null;

        course.Update(title, description, image, CourseStatus.Draft,
            accessType, price, duration, course.TeacherId);

        // Create a new version recording the rollback
        var maxVersion = await _context.CourseVersions
            .Where(v => v.CourseId == course.Id)
            .MaxAsync(v => (int?)v.VersionNumber, cancellationToken) ?? 0;

        var rollbackVersion = new CourseVersion(
            course.Id, version.SnapshotJson, _currentUser.Id,
            $"Rollback to version {version.VersionNumber}");
        rollbackVersion.VersionNumber = maxVersion + 1;
        await _context.CourseVersions.AddAsync(rollbackVersion, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
