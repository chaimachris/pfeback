using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using DeliverWholesale.Infrastructure.Data;

namespace DeliverWholesale.Tests.TestHelpers;

public static class TestDbFactory
{
    public static SqliteConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        return connection;
    }

    public static ApplicationDbContext CreateContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var ctx = new ApplicationDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }
}
