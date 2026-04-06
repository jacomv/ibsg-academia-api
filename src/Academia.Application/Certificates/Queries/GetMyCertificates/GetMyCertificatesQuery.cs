using Academia.Application.Certificates.Dtos;
using MediatR;

namespace Academia.Application.Certificates.Queries.GetMyCertificates;

public record GetMyCertificatesQuery : IRequest<List<CertificateDto>>;
