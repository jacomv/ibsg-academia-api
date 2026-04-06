namespace Academia.Application.Common.Interfaces;

public interface ICertificateGenerator
{
    byte[] Generate(CertificateData data);
}

public record CertificateData(
    string StudentFullName,
    string CourseTitle,
    string CertificateNumber,
    DateTime IssuedAt,
    string? CourseDescription = null
);
