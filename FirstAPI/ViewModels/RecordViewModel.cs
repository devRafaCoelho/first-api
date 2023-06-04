using System.ComponentModel.DataAnnotations;

namespace FirstAPI.ViewModels;

public class RecordViewModel
{
    public int Id { get; set; }

    public int Id_Client { get; set; }

    public string? Description { get; set; }

    public DateTime Due_Date { get; set; }

    public int Value { get; set; }

    public bool Paid_Out { get; set; }

    public string Status { get; set; }

}


