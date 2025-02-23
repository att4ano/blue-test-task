using Bogus;
using FluentMigrator;

namespace Infrastructure.Data.Migrations;

[Migration(202502232210, TransactionBehavior.None)]
public class FakeDataMigration : Migration
{
    public override void Up()
    {
        var faker = new Faker();
        for (int i = 1; i <= 10; i++)
        {
            var user = new
            {
                id = i,
                username = faker.Internet.UserName()
            };
            Insert.IntoTable("users").Row(user);
        }
        for (int i = 1; i <= 5; i++)
        {
            var product = new
            {
                product_id = i,
                product_name = faker.Commerce.ProductName(),
                price = faker.Random.Decimal(10, 1000)
            };
            Insert.IntoTable("products").Row(product);
        }
        
        for (var i = 1; i <= 10; i++)
        {
            var order = new
            {
                order_id = i,
                product_id = faker.Random.Int(1, 5),
                quantity = faker.Random.Int(1, 10),
                paid_at = faker.Date.Past(2),
                amount = faker.Random.Decimal(10, 1000)
            };
            Insert.IntoTable("orders").Row(order);
        }

        for (int i = 1; i <= 50; i++)
        {
            var productView = new
            {
                view_id = i,
                product_id = faker.Random.Int(1, 5), 
                user_id = faker.Random.Int(1, 10), 
                viewed_at = faker.Date.Past(1)
            };
            Insert.IntoTable("product_views").Row(productView);
        }
    }

    public override void Down()
    {
        Delete.FromTable("product_views").AllRows();
        Delete.FromTable("orders").AllRows();
        Delete.FromTable("products").AllRows();
        Delete.FromTable("users").AllRows();
    }
}
