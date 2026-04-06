using Academia.Application.Certificates.Queries.GetCertificatePdf;
using Academia.Application.Certificates.Queries.GetMyCertificates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Academia.WebAPI.Controllers;

[ApiController]
[Route("api/certificates")]
[Authorize]
public class CertificatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CertificatesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetMyCertificates(CancellationToken ct)
        => Ok(await _mediator.Send(new GetMyCertificatesQuery(), ct));

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCertificatePdfQuery(id), ct);
        return File(result.PdfBytes, "application/pdf", result.FileName);
    }
}
