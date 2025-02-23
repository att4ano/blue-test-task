using FluentMigrator;

namespace Infrastructure.Data.Migrations;

[Migration(202502222054, TransactionBehavior.None)]
public class InitialProductMigration : Migration
{
    public override void Up()
    {
        const string sql = @"
            create table products (
                product_id bigint primary key,
                product_name varchar(255) not null,
                price decimal(18, 2) not null
            );

            create index idx_products_product_name on products (product_name);
        ";
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"drop table if exists products;";
        Execute.Sql(sql);
    }
}