using Application.Contracts;
using Application.Mapper;
using Grpc.Core;
using Reports.Service;

namespace Presentation.Controllers;

public class ReportController : ReportService.ReportServiceBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }
    
    public override async Task<GetReportsResponse> GetReport(GetReportRequest request, ServerCallContext context)
    {
        var report = await _reportService.CheckReport(Guid.Parse(request.Id.Guid), context.CancellationToken);
        return new GetReportsResponse
        {
            Report = report.ToProto(),
        };
    }
}