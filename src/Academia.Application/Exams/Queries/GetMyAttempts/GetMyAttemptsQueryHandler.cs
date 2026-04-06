using Academia.Application.Common.Interfaces;
using Academia.Application.Exams.Dtos;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Exams.Queries.GetMyAttempts;

public class GetMyAttemptsQueryHandler : IRequestHandler<GetMyAttemptsQuery, List<AttemptSummaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetMyAttemptsQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<AttemptSummaryDto>> Handle(
        GetMyAttemptsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Grades
            .Where(g => g.UserId == _currentUser.Id && g.ExamId == request.ExamId)
            .OrderBy(g => g.AttemptNumber)
            .AsNoTracking()
            .Select(g => new AttemptSummaryDto(
                g.Id, g.AttemptNumber, g.TotalScore,
                g.IsPassed, g.Status, g.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
