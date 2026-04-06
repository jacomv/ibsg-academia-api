using MediatR;

namespace Academia.Application.Certificates.Queries.GetCertificatePdf;

public record GetCertificatePdfQuery(Guid CertificateId) : IRequest<CertificatePdfResult>;

public record CertificatePdfResult(byte[] PdfBytes, string FileName);
