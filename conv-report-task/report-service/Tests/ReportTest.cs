using Application.Abstractions.Cache;
using Application.Abstractions.Entities;
using Application.Abstractions.Models;
using Application.Abstractions.Persistence;
using Application.Application;
using Application.Contracts;
using Application.Models;
using NSubstitute;
using Xunit;

namespace Tests;

public class ReportTest
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductViewRepository _viewRepository;
    private readonly IReportRepository _reportRepository;
    private readonly ICreatedReportRepository _createdReportRepository;

    public ReportTest()
    {
        _reportRepository = Substitute.For<IReportRepository>();
        _orderRepository = Substitute.For<IOrderRepository>();
        _viewRepository = Substitute.For<IProductViewRepository>();
        _createdReportRepository = Substitute.For<ICreatedReportRepository>();
    }

    [Fact]
    public async Task WithOneOrder()
    {
        // Arrange
        var now = DateTime.Now;

        _orderRepository.QueryAsync(Arg.Any<OrderQuery>(), Arg.Any<CancellationToken>())
            .Returns(new[]
            {
                new OrderEntity(1, 1, 2, now, 100),
            }.ToAsyncEnumerable());

        _viewRepository.QueryAsync(Arg.Any<ViewQuery>(), Arg.Any<CancellationToken>())
            .Returns(new[]
            {
                new ViewEntity(1, 1, now),
            }.ToAsyncEnumerable());

        IReportService reportService = new ReportService(
            _orderRepository, 
            _viewRepository, 
            _reportRepository,
            _createdReportRepository);
        
        Guid reportGuid = Guid.NewGuid();
        var createDto = new CreateReportDto(now, now, 1, reportGuid);

        // Act
        await reportService.CreateReport(createDto, CancellationToken.None);

        // Assert
        await _reportRepository.Received(1)
            .AddAsync(
                Arg.Is<IReadOnlyCollection<InsertReportModel>>(x =>
                    x.First().Id == reportGuid &&
                    x.First().ProductId == 1 &&
                    x.First().StartPeriod.Date == now.Date &&
                    x.First().EndPeriod.Date == now.Date &&
                    x.First().ViewCount == 1 &&
                    x.First().PaymentCount == 1 &&
                    x.First().Ratio == 1.0),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task WithMultipleOrdersAndViews()
    {
        // Arrange
        var now = DateTime.Now;

        _orderRepository.QueryAsync(Arg.Any<OrderQuery>(), Arg.Any<CancellationToken>())
            .Returns(new[]
            {
                new OrderEntity(1, 1, 2, now, 100),
                new OrderEntity(2, 1, 3, now.AddHours(1), 200),
                new OrderEntity(3, 1, 4, now.AddHours(2), 300),
            }.ToAsyncEnumerable());

        _viewRepository.QueryAsync(Arg.Any<ViewQuery>(), Arg.Any<CancellationToken>())
            .Returns(new[]
            {
                new ViewEntity(1, 1, now),
                new ViewEntity(2, 1, now.AddHours(1)),
                new ViewEntity(3, 1, now.AddHours(2)),
                new ViewEntity(4, 1, now.AddHours(3)),
            }.ToAsyncEnumerable());

        IReportService reportService = new ReportService(
            _orderRepository, 
            _viewRepository, 
            _reportRepository, 
            _createdReportRepository);
        
        Guid reportGuid = Guid.NewGuid();
        var createDto = new CreateReportDto(now, now.AddHours(3), 1, reportGuid);

        // Act
        await reportService.CreateReport(createDto, CancellationToken.None);

        // Assert
        await _reportRepository.Received(1)
            .AddAsync(
                Arg.Is<IReadOnlyCollection<InsertReportModel>>(x =>
                    x.First().Id == reportGuid &&
                    x.First().ProductId == 1 &&
                    x.First().StartPeriod.Date == now.Date &&
                    x.First().EndPeriod.Date == now.AddHours(3).Date &&
                    x.First().ViewCount == 4 &&
                    x.First().PaymentCount == 3 &&
                    x.First().Ratio == 4.0 / 3.0),
                Arg.Any<CancellationToken>());
    }
}