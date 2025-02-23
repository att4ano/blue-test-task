using Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Presentation.Dto.Request;
using Presentation.Dto.Response;

namespace Presentation.Controllers;

/// <summary>
/// Контроллер для работы с отчетами.
/// </summary>
[ApiController]
[Route("report")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Создает новый отчет.
    /// </summary>
    /// <param name="request">Данные для создания отчета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор созданного отчета.</returns>
    [HttpPost]
    public async Task<ActionResult<CreateReportResponse>> CreateReport(
        [FromBody] CreateReportRequest request,
        CancellationToken cancellationToken)
    {
        var reportCreationId = await _reportService.CreateReport(
            request.StartPeriod, 
            request.EndPeriod,
            request.ProductId, 
            cancellationToken);

        return Ok(new CreateReportResponse(reportCreationId));
    }

    /// <summary>
    /// Проверяет статус отчета по ID.
    /// </summary>
    /// <param name="id">Идентификатор отчета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Статус отчета.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CheckReportResponse>> CheckReport(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var reportResult = await _reportService.CheckReport(id, cancellationToken);
        var response = new CheckReportResponse(reportResult);
        
        return Ok(response);
    } 
}
