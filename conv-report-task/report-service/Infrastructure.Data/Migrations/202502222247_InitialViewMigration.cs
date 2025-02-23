using FluentMigrator;

namespace Infrastructure.Data.Migrations;

[Migration(202502222247, TransactionBehavior.None)]
public class InitialViewMigration : Migration
{
    public override void Up()
    {
        const string sql = @"
            create table product_views (
                view_id bigint primary key,
                product_id bigint not null,
                user_id bigint not null,
                viewed_at timestamp not null,
                constraint fk_product foreign key (product_id) references products (product_id),
                constraint fk_user foreign key (user_id) references users (id)
            );

            create index idx_product_views_product_id on product_views (product_id);
            create index idx_product_views_viewed_at on product_views (viewed_at);
";

        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"
            drop table if exists product_views;
            drop index if exists idx_product_views_product_id;
            drop index if exists idx_product_views_viewed_at;
        ";
        
        Execute.Sql(sql);
    }
}