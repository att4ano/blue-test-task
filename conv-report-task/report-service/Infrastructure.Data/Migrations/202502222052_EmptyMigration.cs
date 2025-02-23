using FluentMigrator;

namespace Infrastructure.Data.Migrations;

[Migration(202502222052, TransactionBehavior.None)]
public class EmptyMigration : Migration
{
    public override void Up()
    {
    }

    public override void Down()
    {
    }
}