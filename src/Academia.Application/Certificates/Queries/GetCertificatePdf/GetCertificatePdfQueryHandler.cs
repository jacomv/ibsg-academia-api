using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Certificates.Queries.GetCertificatePdf;

public class GetCertificatePdfQueryHandler
    : IRequestHandler<GetCertificatePdfQuery, CertificatePdfResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICertificateGenerator _generator;
    private readonly ICurrentUser _currentUser;

    public GetCertificatePdfQueryHandler(
        IApplicationDbContext context,
        ICertificateGenerator generator,
        ICurrentUser currentUser)
    {
        _context = context;
        _generator = generator;
        _currentUser = currentUser;
    }

    public async Task<CertificatePdfResult> Handle(
        GetCertificatePdfQuery request, CancellationToken cancellationToken)
    {
        var certificate = await _context.Certificates
            .Include(c => c.User)
            .Include(c => c.Course)
            .FirstOrDefaultAsync(c =>
                c.Id == request.CertificateId &&
                c.UserId == _currentUser.Id, cancellationToken);

        if (certificate is null)
            throw new NotFoundException("Certificate", request.CertificateId);

        var pdfBytes = _generator.Generate(new CertificateData(
            StudentFullName: certificate.User.FullName,
            CourseTitle: certificate.Course.Title,
            CertificateNumber: certificate.CertificateNumber,
            IssuedAt: certificate.IssuedAt,
            CourseDescription: certificate.Course.Description
        ));

        var fileName = $"Certificado_{certificate.Course.Title.Replace(" ", "_")}_{certificate.CertificateNumber}.pdf";

        return new CertificatePdfResult(pdfBytes, fileName);
    }
}
