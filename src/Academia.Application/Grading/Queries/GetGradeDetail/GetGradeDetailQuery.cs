using Academia.Application.Grading.Dtos;
using MediatR;

namespace Academia.Application.Grading.Queries.GetGradeDetail;

public record GetGradeDetailQuery(Guid GradeId) : IRequest<GradeDetailDto>;
