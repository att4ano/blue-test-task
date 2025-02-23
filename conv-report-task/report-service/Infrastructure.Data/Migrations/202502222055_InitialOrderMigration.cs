using FluentMigrator;

namespace Infrastructure.Data.Migrations;

[Migration(202502222055, TransactionBehavior.None)]
public class InitialOrderMigration : Migration
{
    public override void Up()
    {
        const string sql = @"create table orders (
                                order_id bigint primary key,
                                product_id bigint not null,
                                quantity bigint not null,
                                paid_at timestamp not null,
                                amount decimal(18, 2),
                                constraint fk_product foreign key (product_id) references products (product_id)
                            );
                            create index idx_orders_product_id on orders (product_id);";
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"
            drop table if exists orders;
            drop index if exists idx_orders_product_id;
        ";
        
        Execute.Sql(sql);
    }
}