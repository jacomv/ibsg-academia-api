using Academia.Domain.Common;
using Academia.Domain.Enums;

namespace Academia.Domain.Entities;

public class EditorialReview : BaseEntity
{
    private EditorialReview() { }

    public EditorialReview(Guid courseId, Guid reviewerId, EditorialDecision decision, string? comment)
    {
        CourseId = courseId;
        ReviewerId = reviewerId;
        Decision = decision;
        Comment = comment;
    }

    public Guid CourseId { get; private set; }
    public Guid ReviewerId { get; private set; }
    public EditorialDecision Decision { get; private set; }
    public string? Comment { get; private set; }

    public Course Course { get; private set; } = default!;
    public User Reviewer { get; private set; } = default!;
}
