using MediatR;

namespace Academia.Application.Courses.Commands.SubmitForReview;

public record SubmitForReviewCommand(Guid CourseId) : IRequest;
