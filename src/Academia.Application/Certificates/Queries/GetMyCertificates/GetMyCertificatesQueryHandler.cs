using Academia.Application.Certificates.Dtos;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Certificates.Queries.GetMyCertificates;

public class GetMyCertificatesQueryHandler
    : IRequestHandler<GetMyCertificatesQuery, List<CertificateDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetMyCertificatesQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<CertificateDto>> Handle(
        GetMyCertificatesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Certificates
            .Include(c => c.Course)
            .Where(c => c.UserId == _currentUser.Id)
            .OrderByDescending(c => c.IssuedAt)
            .AsNoTracking()
            .Select(c => new CertificateDto(
                c.Id, c.CourseId, c.Course.Title,
                c.Course.Image, c.CertificateNumber, c.IssuedAt))
            .ToListAsync(cancellationToken);
    }
}
