using FirstAPI.Repositories;
using FirstAPI.Repositories.Contracts;
using System.Data.SqlClient;

namespace FirstAPI.Extensions;

public static class DependenciesExtension
{
    public static void AddSqlConnection(
        this IServiceCollection services,
        string connectionString)
    {

        services.AddScoped<SqlConnection>(x
            => new SqlConnection(connectionString));
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
    }
}

