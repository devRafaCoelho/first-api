using FirstAPI.Models;

namespace FirstAPI.ViewModels;

public class ListClientsViewModel
{
    public int Total { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public List<ClientViewModel> Clients { get; set; }
}

