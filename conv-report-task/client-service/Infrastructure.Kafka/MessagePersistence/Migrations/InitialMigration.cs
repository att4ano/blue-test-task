using FluentMigrator;

namespace Infrastructure.Kafka.MessagePersistence.Migrations;

[Migration(202502211311, TransactionBehavior.None)]
public class InitialMigration : Migration
{
    public override void Up()
    {
        const string sql = @"
        create type persisted_message_state as enum
        (
            'pending',
            'completed'
        );

        create table persisted_messages
        (
            persisted_message_id bigint primary key generated always as identity ,
            persisted_message_name text not null ,
            persisted_message_created_at timestamp with time zone not null ,
            persisted_message_state persisted_message_state not null default 'pending',
            persisted_message_key jsonb not null ,
            persisted_message_value jsonb not null 
        );

        create index persisted_message_idx on persisted_messages(persisted_message_name, persisted_message_state);
        ";
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"
                            drop index if exists persisted_message_idx;
                            drop table if exists persisted_messages;
                            drop type if exists persisted_message_state;
                            ";
        
        Execute.Sql(sql);
    }
}