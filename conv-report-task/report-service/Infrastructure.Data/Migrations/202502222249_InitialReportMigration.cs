using FluentMigrator;

namespace Infrastructure.Data.Migrations;

[Migration(202502222249, TransactionBehavior.None)]
public class InitialReportMigration : Migration
{
    public override void Up()
    {
        const string sql = @"
            create table reports (
                report_id uuid primary key,
                product_id bigint not null,
                start_period timestamp not null,
                end_period timestamp not null,
                view_count int not null,
                payment_count int not null,
                view_to_payment_ratio decimal(18, 2) not null,
                created_at timestamp not null
            );

            create index idx_reports_product_id on reports (product_id);
            create index idx_reports_created_at on reports (created_at);
        ";
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"
            drop table if exists reports;
            drop index if exists idx_reports_product_id;
            drop index if exists idx_reports_created_at;
        ";
        
        Execute.Sql(sql);
    }
}