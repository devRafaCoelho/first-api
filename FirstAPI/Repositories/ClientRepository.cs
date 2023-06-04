using Dapper;
using FirstAPI.Models;
using FirstAPI.Repositories.Contracts;
using FirstAPI.ViewModels;
using Microsoft.VisualBasic;
using System.Data;
using System.Data.SqlClient;

namespace FirstAPI.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly string? _connectionString;

    public ClientRepository(IConfiguration configuration)
        => _connectionString = configuration.GetConnectionString("API");

    public async Task<Client> AddClientAsync(Client model)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = @"INSERT INTO [clients] (
                                    Name,
                                    Email,
                                    CPF,
                                    Phone,
                                    Address,
                                    Complement,
                                    Zip_Code,
                                    District,
                                    City,
                                    UF
                                )
                                OUTPUT INSERTED.Id
                                VALUES (
                                    @Name,
                                    @Email,
                                    @CPF,
                                    @Phone,
                                    @Address,
                                    @Complement,
                                    @Zip_Code,
                                    @District,
                                    @City,
                                    @UF
                                )";
            ;

            var parameters = new
            {
                model.Name,
                model.Email,
                model.CPF,
                model.Phone,
                model.Address,
                model.Complement,
                model.Zip_Code,
                model.District,
                model.City,
                model.UF
            };

            var result = await connection.QueryFirstOrDefaultAsync<Client>(sql, parameters);

            return result;
        }
    }

    public async Task<int> GetCountClientsAsync()
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "SELECT COUNT(*) FROM clients";

            var result = await connection.ExecuteScalarAsync<int>(sql);

            return result;
        }
    }

    public async Task<List<ClientViewModel>> GetAllClientsAsync(int skip, int take)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {

            const string sql = @"SELECT
                                   clients.id,
                                   clients.name,
                                   clients.email,
                                   clients.phone,
                                   clients.cpf,
                                   CASE
                                     WHEN EXISTS (
                                       SELECT 1
                                       FROM records
                                       WHERE records.id_client = clients.id
                                         AND records.due_date < GETDATE()
                                         AND records.paid_out <> 1
                                     ) THEN 'Inadimplente'
                                     ELSE 'Em dia'
                                   END AS status
                                 FROM
                                   clients
                                 LEFT JOIN
                                   records ON clients.id = records.id_client
                                 GROUP BY
                                   clients.id,
                                   clients.name,
                                   clients.email,
                                   clients.phone,
                                   clients.cpf
                                 ORDER BY
                                   clients.id
                                 OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;";

            var parameters = new
            {
                skip,
                take
            };

            var result = await connection.QueryAsync<ClientViewModel>(sql, parameters);

            return result.ToList();
        }
    }

    public async Task<ClientViewModel> GetClientByIdAsync(int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = @"SELECT
                                   clients.id,
                                   clients.name,
                                   clients.email,
                                   clients.phone,
                                   clients.cpf,
                                   CASE
                                     WHEN records.paid_out = 1 THEN 'Em dia'
                                     WHEN records.due_date < GETDATE() THEN 'Inadimplente'
                                     ELSE 'Em dia'
                                   END AS status
                                 FROM
                                   clients
                                 LEFT JOIN
                                   records ON clients.id = records.id_client
                                 WHERE clients.id = @id;";

            var parameters = new { id };

            var result = await connection.QueryFirstOrDefaultAsync<ClientViewModel>(sql, parameters);

            return result;
        }
    }

    public async Task<ClientViewModel> GetClientByEmailAsync(string email)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = @"SELECT
                                   clients.id,
                                   clients.name,
                                   clients.email,
                                   clients.phone,
                                   clients.cpf,
                                   CASE
                                     WHEN records.paid_out = 1 THEN 'Em dia'
                                     WHEN records.due_date < GETDATE() THEN 'Inadimplente'
                                     ELSE 'Em dia'
                                   END AS status
                                 FROM
                                   clients
                                 LEFT JOIN
                                   records ON clients.id = records.id_client
                                 WHERE clients.email = @email;";

            var parameters = new { email };

            var result = await connection.QueryFirstOrDefaultAsync<ClientViewModel>(sql, parameters);

            return result;
        }
    }

    public async Task<ClientViewModel> GetClientByCPFAsync(string cpf)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = @"SELECT
                                   clients.id,
                                   clients.name,
                                   clients.email,
                                   clients.phone,
                                   clients.cpf,
                                   CASE
                                     WHEN records.paid_out = 1 THEN 'Em dia'
                                     WHEN records.due_date < GETDATE() THEN 'Inadimplente'
                                     ELSE 'Em dia'
                                   END AS status
                                 FROM
                                   clients
                                 LEFT JOIN
                                   records ON clients.id = records.id_client
                                 WHERE clients.cpf = @cpf;";

            var parameters = new { cpf };

            var result = await connection.QueryFirstOrDefaultAsync<ClientViewModel>(sql, parameters);

            return result;
        }
    }

    public async Task<bool> UpdateClientByIdAsync(Client model, int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = @"UPDATE [clients]
                                 SET Name = @Name,
                                     Email = @Email,
                                     CPF = @CPF,
                                     Phone = @Phone,
                                     Address = @Address,
                                     Complement = @Complement,
                                     Zip_Code = @Zip_Code,
                                     District = @District,
                                     City = @City,
                                     UF = @UF
                                 WHERE id = @id";

            var parameters = new Client
            {
                Id = id,
                Name = model.Name,
                Email = model.Email,
                CPF = model.CPF,
                Phone = model.Phone,
                Address = model.Address,
                Complement = model.Complement,
                Zip_Code = model.Zip_Code,
                District = model.District,
                City = model.City,
                UF = model.UF
            };

            await connection.ExecuteAsync(sql, parameters);

            return true;
        }
    }

    public async Task<bool> DeleteClientByIdAsync(int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "DELETE FROM [clients] WHERE id = @id;";

            var parameters = new { id };

            await connection.ExecuteAsync(sql, parameters);

            return true;
        }
    }

}

