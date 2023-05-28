using FirstAPI.Models;
using FirstAPI.ViewModels;

namespace FirstAPI.Repositories.Contracts;

public interface IClientRepository
{
    Task<Client> AddClientAsync(Client model);

    Task<int> GetCountClientsAsync();

    Task<List<ClientViewModel>> GetAllClientsAsync(int skip, int take);

    Task<ClientViewModel> GetClientByIdAsync(int id);

    Task<ClientViewModel> GetClientByEmailAsync(string email);

    Task<ClientViewModel> GetClientByCPFAsync(string cpf);

    Task<bool> UpdateClientByIdAsync(Client model, int id);

    Task<bool> DeleteClientByIdAsync(int id);
}

