namespace FirstAPI.Models;

public class Record
{
    public int Id_Client { get; set; }

    public string? Description { get; set; }

    public DateTime Due_Date { get; set; }

    public int Value { get; set; }

    public bool Paid_Out { get; set; }
}

