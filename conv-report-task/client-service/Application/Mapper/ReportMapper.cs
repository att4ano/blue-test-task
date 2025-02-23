using Application.Models;
using Reports.Service;

namespace Application.Mapper;

public static class ReportMapper
{
    public static Report ToModel(this ReportProto proto)
        => new Report(proto.RatioViewsPayments, proto.PaymentCount);
}