using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Courses.Queries.GetEditorialReviews;

public record GetEditorialReviewsQuery(Guid CourseId) : IRequest<List<EditorialReviewDto>>;

public record EditorialReviewDto(
    Guid Id,
    Guid ReviewerId,
    string ReviewerName,
    EditorialDecision Decision,
    string? Comment,
    DateTime CreatedAt
);
