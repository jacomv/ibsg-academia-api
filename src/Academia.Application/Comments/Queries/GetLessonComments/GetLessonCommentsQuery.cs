using Academia.Application.Comments.Dtos;
using MediatR;

namespace Academia.Application.Comments.Queries.GetLessonComments;

public record GetLessonCommentsQuery(Guid LessonId) : IRequest<List<CommentDto>>;
