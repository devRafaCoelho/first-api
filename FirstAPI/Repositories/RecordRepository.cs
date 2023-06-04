using Dapper;
using FirstAPI.Models;
using FirstAPI.Repositories.Contracts;
using FirstAPI.ViewModels;
using Microsoft.VisualBasic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Sockets;

namespace FirstAPI.Repositories;

public class RecordRepository : IRecordRepository
{
    private readonly string? _connectionString;

    public RecordRepository(IConfiguration configuration)
        => _connectionString = configuration.GetConnectionString("API");

    public async Task<Record> AddRecordAsync(Record model)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = @"INSERT INTO [records] (
                                 Id_Client,
                                 Description,
                                 Due_Date,
                                 Value,
                                 Paid_Out                                    
                             )
                             OUTPUT INSERTED.Id
                             VALUES (
                                 @Id_Client,
                                 @Description,
                                 @Due_Date,
                                 @Value,
                                 @Paid_Out                                   
                             )";

            var parameters = new
            {
                model.Id_Client,
                model.Description,
                model.Due_Date,
                model.Value,
                model.Paid_Out
            };

            var result = await connection.QueryFirstOrDefaultAsync<Record>(sql, parameters);

            return result;
        }
    }

    public async Task<int> GetCountRecordsAsync()
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "SELECT COUNT(*) FROM records";

            var result = await connection.ExecuteScalarAsync<int>(sql);

            return result;
        }
    }

    public async Task<List<RecordViewModel>> GetAllRecordsAsync(int skip, int take)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = @"SELECT
                                   records.id,
                                   records.id_client,
                                   records.description,
                                   records.due_date,
                                   records.value,
                                   records.paid_out,                               
                                   CASE
                                     WHEN records.due_date < GETDATE() AND records.paid_out = 0 THEN 'Vencida'
                                     WHEN records.due_date > GETDATE() AND records.paid_out = 0 THEN 'Pendente'
                                     ELSE 'Paga'
                                   END AS status
                                 FROM
                                   records
                                 LEFT JOIN
                                   clients ON records.id_client = clients.id
                                 ORDER BY records.id OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;";

            var parameters = new
            {
                skip,
                take
            };

            var result = await connection.QueryAsync<RecordViewModel>(sql, parameters);

            return result.ToList();
        }
    }

    public async Task<RecordViewModel> GetRecordByIdAsync(int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = @"SELECT
                                   records.id,
                                   records.id_client,
                                   records.description,
                                   records.due_date,
                                   records.value,
                                   CASE
                                     WHEN records.due_date < GETDATE() AND records.paid_out = 0 THEN 'Vencida'
                                     WHEN records.due_date > GETDATE() AND records.paid_out = 0 THEN 'Pendente'
                                     ELSE 'Paga'
                                   END AS status
                                 FROM
                                   records
                                 WHERE
                                   records.id = @id";

            var parameters = new { id };

            var result = await connection.QueryFirstOrDefaultAsync<RecordViewModel>(sql, parameters);

            return result;
        }
    }

    public async Task<bool> UpdateRecordByIdAsync(Record model, int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = @"UPDATE [records]
                                 SET Description = @Description,
                                     Due_Date = @Due_Date,
                                     Value = @Value,
                                     Paid_Out = @Paid_Out                                   
                                 WHERE id = @id";

            var parameters = new Record
            {
                Id = id,
                Description = model.Description,
                Due_Date = model.Due_Date,
                Value = model.Value,
                Paid_Out = model.Paid_Out
            };

            await connection.ExecuteAsync(sql, parameters);

            return true;
        }
    }

    public async Task<bool> DeleteRecordByIdAsync(int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "DELETE FROM [records] WHERE id = @id;";

            var parameters = new { id };

            await connection.ExecuteAsync(sql, parameters);

            return true;
        }
    }
}

