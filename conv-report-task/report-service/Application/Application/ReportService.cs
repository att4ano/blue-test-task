using System.Transactions;
using Application.Abstractions.Cache;
using Application.Abstractions.Models;
using Application.Abstractions.Persistence;
using Application.Contracts;
using Application.Mapper;
using Application.Models;

namespace Application.Application;

public class ReportService : IReportService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductViewRepository _productViewRepository;
    private readonly IReportRepository _reportRepository;
    private readonly ICreatedReportRepository _createdReportRepository;

    public ReportService(
        IOrderRepository orderRepository,
        IProductViewRepository productViewRepository,
        IReportRepository reportRepository,
        ICreatedReportRepository createdReportRepository)
    {
        _orderRepository = orderRepository;
        _productViewRepository = productViewRepository;
        _reportRepository = reportRepository;
        _createdReportRepository = createdReportRepository;
    }

    public async Task CreateReport(CreateReportDto reportDto, CancellationToken cancellationToken)
    {
        var orderQuery = new OrderQuery([], [reportDto.ProductId], reportDto.StartPeriod, reportDto.EndPeriod, null);
        var viewQuery = new ViewQuery([], [reportDto.ProductId], reportDto.StartPeriod, reportDto.EndPeriod);

        using var transaction = CreateTransactionScope();

        var orders = await _orderRepository.QueryAsync(orderQuery, cancellationToken).ToListAsync(cancellationToken);
        var views = await _productViewRepository.QueryAsync(viewQuery, cancellationToken)
            .ToListAsync(cancellationToken);

        var viewCount = views.Count;
        var paymentCount = orders.Count;
        var viewToPaymentRatio = paymentCount == 0 ? 0 : (double)viewCount / paymentCount;

        var insertModel = new InsertReportModel(
            Id: reportDto.ReportId,
            ProductId: reportDto.ProductId,
            StartPeriod: reportDto.StartPeriod,
            EndPeriod: reportDto.EndPeriod,
            ViewCount: viewCount,
            PaymentCount: paymentCount,
            Ratio: viewToPaymentRatio);

        await _reportRepository.AddAsync([insertModel], cancellationToken);

        transaction.Complete();
    }

    public async Task<Report> CheckReport(Guid reportId, CancellationToken cancellationToken)
    {
        var cachedReport = await _createdReportRepository.Get(reportId, cancellationToken);
        if (cachedReport != null)
        {
            return cachedReport.ToModel();
        }

        var query = new ReportQuery([reportId]);
        var report = await _reportRepository
            .QueryAsync(query, cancellationToken)
            .SingleAsync(cancellationToken);

        await _createdReportRepository.Add(report, cancellationToken);
        return report.ToModel();
    }

    private static TransactionScope CreateTransactionScope(
        IsolationLevel level = IsolationLevel.ReadCommitted)
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = level,
                Timeout = TimeSpan.FromSeconds(5),
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }
}