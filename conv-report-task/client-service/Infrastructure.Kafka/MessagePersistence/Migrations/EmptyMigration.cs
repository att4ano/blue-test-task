using FluentMigrator;

namespace Infrastructure.Kafka.MessagePersistence.Migrations;

[Migration(202502211310, TransactionBehavior.None)]
public class EmptyMigration : Migration
{
    public override void Up()
    {
    }

    public override void Down()
    {
    }
}