using FirstAPI.Models;
using FirstAPI.ViewModels;

namespace FirstAPI.Repositories.Contracts;

public interface IRecordRepository
{
    Task<Record> AddRecordAsync(Record model);

    Task<int> GetCountRecordsAsync();

    Task<List<RecordViewModel>> GetAllRecordsAsync(int skip, int take);

    Task<RecordViewModel> GetRecordByIdAsync(int id);

    Task<bool> UpdateRecordByIdAsync(Record model, int id);

    Task<bool> DeleteRecordByIdAsync(int id);
}

