namespace FirstAPI.ViewModels;

public class ListRecordsViewModel
{
    public int Total { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public List<RecordViewModel> Records { get; set; }
}

