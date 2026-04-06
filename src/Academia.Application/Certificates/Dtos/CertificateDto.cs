namespace Academia.Application.Certificates.Dtos;

public record CertificateDto(
    Guid Id,
    Guid CourseId,
    string CourseTitle,
    string? CourseImage,
    string CertificateNumber,
    DateTime IssuedAt
);
