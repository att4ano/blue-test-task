using Application.Abstractions.Entities;
using Application.Models;
using Reports.Service;

namespace Application.Mapper;

public static class ReportMapper
{
    public static Report ToModel(this ReportEntity entity)
        => new Report(
            entity.ProductId, 
            entity.StartPeriod, 
            entity.EndPeriod, 
            entity.Ratio, 
            entity.PaymentCount,
            entity.ViewCount);


    public static ReportProto ToProto(this Report report)
        => new ReportProto
        {
            RatioViewsPayments = report.Ratio,
            PaymentCount = report.PaymentCount,
        };
}