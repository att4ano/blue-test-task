using FluentMigrator;

namespace Infrastructure.Data.Migrations;

[Migration(202502222053, TransactionBehavior.None)]
public class InitialUserMigration : Migration
{
    public override void Up()
    {
        const string sql = @"create table users (
                             id bigserial primary key,
                             username varchar(255) not null
                             );";
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"drop table if exists users";
        Execute.Sql(sql);
    }
}